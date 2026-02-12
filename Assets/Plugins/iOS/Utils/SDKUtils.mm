#import "SDKUtils.h"
#import <CommonCrypto/CommonDigest.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>
#import <Foundation/Foundation.h>

#include <sys/types.h>
#include <sys/sysctl.h>

@implementation NSString (change)
- (NSString*)md532BitLower
{
	const char *cStr = [self UTF8String];
	unsigned char result[16];
	
	NSNumber *num = [NSNumber numberWithUnsignedLong:strlen(cStr)];
	CC_MD5( cStr,[num intValue], result );
	
	return [[NSString stringWithFormat:
			 @"%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X",
			 result[0], result[1], result[2], result[3],
			 result[4], result[5], result[6], result[7],
			 result[8], result[9], result[10], result[11],
			 result[12], result[13], result[14], result[15]
			 ] lowercaseString];
}

- (NSString*)md532BitUpper
{
	const char *cStr = [self UTF8String];
	unsigned char result[16];
	
	NSNumber *num = [NSNumber numberWithUnsignedLong:strlen(cStr)];
	CC_MD5( cStr,[num intValue], result );
	
	return [[NSString stringWithFormat:
			 @"%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X",
			 result[0], result[1], result[2], result[3],
			 result[4], result[5], result[6], result[7],
			 result[8], result[9], result[10], result[11],
			 result[12], result[13], result[14], result[15]
			 ] uppercaseString];
}
@end

char* SDKMakeStringCopy( const char* string)
{
	if (NULL == string) {
		return NULL;
	}
	char* res = (char*)malloc(strlen(string)+1);
	strcpy(res, string);
	return res;
}

BOOL stringIsEmpty(NSString *string)
{
	if([string isKindOfClass:[NSNull class]]){
		return YES;
	}
	
	if (string == nil) {
		return YES;
	}
	
	if ([string isEqualToString:@"(null)"]) {
		return YES;
	}
	
	NSString *text = [string stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
	
	if ([text length] == 0) {
		return YES;
	}
	
	return NO;
}

@implementation SDKUtils

static SDKUtils* _instance = nil;

+(instancetype) shareInstance
{
	static dispatch_once_t onceToken;
	dispatch_once(&onceToken, ^{
		_instance = [[super allocWithZone:NULL] init];
	});
	
	return _instance;
}

+(id) allocWithZone:(struct _NSZone *)zone
{
	return [SDKUtils shareInstance];
}

+(id) copyWithZone:(struct _NSZone *)zone
{
	return [SDKUtils shareInstance];
}

- (void)sdkLog:(NSString *)format, ... NS_FORMAT_FUNCTION(1, 2) {
	if (!isSDKLog) {
		return;
	}
	va_list args;
	va_start(args, format);
	NSString *rst = [[NSString alloc] initWithFormat:format arguments:args];
	va_end(args);
	NSLog(@"sdkLog %@",rst);
	if (rst != nil) {
		UnitySendMessage("SDKInterface", "LogSDK", [rst UTF8String]);
	}
}

-(NSString *)PackageParam:(NSString *)value, ...
{
	NSString* stringAll = [[NSString alloc] init];
	NSMutableArray *argsArray = [[NSMutableArray alloc] init];
	
	va_list params; //定义一个指向个数可变的参数列表指针;
	va_start(params, value);//va_start 得到第一个可变参数地址,
	NSString* arg;
	if (value) {
		//将第一个参数添加到array
		NSString* prev = value;
		[argsArray addObject:prev];
		//va_arg 指向下一个参数地址
		//这里是问题的所在 网上的例子，没有保存第一个参数地址，后边循环，指针将不会在指向第一个参数
		while( (arg = va_arg(params, NSString*)) )
		{
			if (arg)
			{
				[argsArray addObject:arg];
			}
		}
		
		//置空
		va_end(params);
		
		//这里循环 将看到所有参数
		NSString* strLengthAll = [NSString stringWithFormat:@"%02lu", (unsigned long)[argsArray count]];
		stringAll = [stringAll stringByAppendingString:strLengthAll];
		
		for (NSString* string in argsArray) {
			//NSLog(@"%@",string);
			if (string != nil) {
				NSString* strLength = [NSString stringWithFormat:@"%04lu", (unsigned long)[string length]];
				stringAll = [stringAll stringByAppendingString:strLength];
				stringAll = [stringAll stringByAppendingString:string];
			} else {
				NSString* strLength = [NSString stringWithFormat:@"%04lu", (unsigned long)0];
				stringAll = [stringAll stringByAppendingString:strLength];
			}
		}
	}
	
	NSLog(@"PackageParam %@",stringAll);
	
	return stringAll;
}

/**
 * url:请求地址
 * verb:请求方式
 * parameters:请求参数
 */
-(NSDictionary *)sendRequestTo:(NSURL *)url usingVerb:(NSString *)verb withParameters:(NSDictionary *)parameters{
	@try {
		NSData *body = nil;
		NSMutableString *params = nil;
		NSString *contentType = @"text/html; charset=utf-8";
		NSURL *finalURL = url;
		if(nil != parameters){
			params = [[NSMutableString alloc] init];
			for(id key in parameters){
				NSString *encodedkey = [key stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
				
				CFStringRef value = (__bridge CFStringRef)[[parameters objectForKey:key] copy];
				CFStringRef encodedValue = CFURLCreateStringByAddingPercentEscapes(kCFAllocatorDefault, value,NULL,(CFStringRef)@";/?:@&=+$", kCFStringEncodingUTF8);
				[params appendFormat:@"%@=%@&", encodedkey, encodedValue];
				CFRelease(value);
				CFRelease(encodedValue);
			}
			[params deleteCharactersInRange:NSMakeRange([params length] - 1, 1)];
		}
		//
		if([verb isEqualToString:@"POST"]){
			contentType = @"application/x-www-form-urlencoded; charset=utf-8";
			body = [params dataUsingEncoding:NSUTF8StringEncoding];
		}else{
			if(nil != parameters){
				NSString *urlWithParams = [[url absoluteString] stringByAppendingFormat:@"?%@", params];
				finalURL = [NSURL URLWithString:urlWithParams];
			}
		}
		NSMutableDictionary *headers = [[NSMutableDictionary alloc] init];
		[headers setValue:contentType forKey:@"Content-Type"];
		[headers setValue:@"text/html" forKey:@"Accept"];
		[headers setValue:@"no-cache" forKey:@"Cache-Control"];
		[headers setValue:@"no-cache" forKey:@"Pragma"];
		[headers setValue:@"close" forKey:@"Connection"];
		NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:finalURL cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:60.0];
		
		if (request == nil) {
			return nil;
		}
		
		[request setHTTPMethod:verb];
		[request setAllHTTPHeaderFields:headers];
		if(nil != parameters){
			[request setHTTPBody:body];
		}
		params = nil;
		//
		NSURLResponse *response;
		NSError *error = nil;
		
		NSData *responseData = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
		
		if (error != nil) {
			NSLog(@"something is wrong: %@", [error description]);
			return nil;
		}
		
		if(responseData == nil){
			NSLog(@"responseData is empty");
			return nil;
		}
		
		NSError* nsError = nil;
		NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:responseData options:(NSJSONReadingOptions) kNilOptions error:&nsError];
		
		return dict;
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"sendRequestTo-finally");
	}
	
	return nil;
}

- (NSString*)getPlatformParam1
{
	return [self getGameIDWithLanguage];
}

- (NSString*)getPlatformParam2
{
	return mPlatformParam2;
}

- (NSString*)getPlatformParam3
{
	return mPlatformParam3;
}

- (NSString*)getCurrentDevice
{
	int mib[2];
	size_t len;
	char *machine;
	
	mib[0] = CTL_HW;
	mib[1] = HW_MACHINE;
	sysctl(mib, 2, NULL, &len, NULL, 0);
	machine = (char*)malloc(len);
	sysctl(mib, 2, machine, &len, NULL, 0);
	
	NSString *platform = [NSString stringWithCString:machine encoding:NSASCIIStringEncoding];
	free(machine);
	
	if ([platform isEqualToString:@"iPhone1,1"]) return @"iPhone 2G";
	if ([platform isEqualToString:@"iPhone1,2"]) return @"iPhone 3G";
	if ([platform isEqualToString:@"iPhone2,1"]) return @"iPhone 3GS";
	if ([platform isEqualToString:@"iPhone3,1"]) return @"iPhone 4";
	if ([platform isEqualToString:@"iPhone3,2"]) return @"iPhone 4";
	if ([platform isEqualToString:@"iPhone3,3"]) return @"iPhone 4";
	if ([platform isEqualToString:@"iPhone4,1"]) return @"iPhone 4s";
	if ([platform isEqualToString:@"iPhone5,1"]) return @"iPhone 5";
	if ([platform isEqualToString:@"iPhone5,2"]) return @"iPhone 5";
	if ([platform isEqualToString:@"iPhone5,3"]) return @"iPhone 5c";
	if ([platform isEqualToString:@"iPhone5,4"]) return @"iPhone 5c";
	if ([platform isEqualToString:@"iPhone6,1"]) return @"iPhone 5s";
	if ([platform isEqualToString:@"iPhone6,2"]) return @"iPhone 5s";
	if ([platform isEqualToString:@"iPhone7,2"]) return @"iPhone 6";
	if ([platform isEqualToString:@"iPhone7,1"]) return @"iPhone 6 Plus";
	if ([platform isEqualToString:@"iPhone8,1"]) return @"iPhone 6s";
	if ([platform isEqualToString:@"iPhone8,2"]) return @"iPhone 6s Plus";
	if ([platform isEqualToString:@"iPhone8,3"]) return @"iPhone SE";
	if ([platform isEqualToString:@"iPhone8,4"]) return @"iPhone SE";
	if ([platform isEqualToString:@"iPhone9,1"]) return @"iPhone 7";
	if ([platform isEqualToString:@"iPhone9,2"]) return @"iPhone 7 Plus";
	if ([platform isEqualToString:@"iPhone10,1"]) return @"iPhone 8";
	if ([platform isEqualToString:@"iPhone10,4"]) return @"iPhone 8";
	if ([platform isEqualToString:@"iPhone10,2"]) return @"iPhone 8 Plus";
	if ([platform isEqualToString:@"iPhone10,5"]) return @"iPhone 8 Plus";
	if ([platform isEqualToString:@"iPhone10,3"]) return @"iPhone X";
	if ([platform isEqualToString:@"iPhone10,6"]) return @"iPhone X";
	if ([platform isEqualToString:@"iPhone11,8"]) return @"iPhone XR";
	if ([platform isEqualToString:@"iPhone11,2"]) return @"iPhone XS";
	if ([platform isEqualToString:@"iPhone11,6"]) return @"iPhone XS Max";
	if ([platform isEqualToString:@"iPhone11,4"]) return @"iPhone XS Max";
	if ([platform isEqualToString:@"iPhone12,1"]) return @"iPhone 11";
	if ([platform isEqualToString:@"iPhone12,3"]) return @"iPhone 11 Pro";
	if ([platform isEqualToString:@"iPhone12,5"]) return @"iPhone 11 Pro Max";
	if ([platform isEqualToString:@"iPhone12,8"]) return @"iPhone SE 2";
	if ([platform isEqualToString:@"iPhone13,1"]) return @"iPhone 12 mini";
	if ([platform isEqualToString:@"iPhone13,2"]) return @"iPhone 12";
	if ([platform isEqualToString:@"iPhone13,3"]) return @"iPhone 12 Pro";
	if ([platform isEqualToString:@"iPhone13,4"]) return @"iPhone 12 Pro Max";
	if ([platform isEqualToString:@"iPhone14,4"]) return @"iPhone 13 mini";
	if ([platform isEqualToString:@"iPhone14,5"]) return @"iPhone 13";
	if ([platform isEqualToString:@"iPhone14,2"]) return @"iPhone 13 Pro";
	if ([platform isEqualToString:@"iPhone14,3"]) return @"iPhone 13 Pro Max";
	if ([platform isEqualToString:@"iPhone14,6"]) return @"iPhone SE 3";
	if ([platform isEqualToString:@"iPhone14,7"]) return @"iPhone 14";
	if ([platform isEqualToString:@"iPhone14,8"]) return @"iPhone 14 Plus";
	if ([platform isEqualToString:@"iPhone15,2"]) return @"iPhone 14 Pro";
	if ([platform isEqualToString:@"iPhone15,3"]) return @"iPhone 14 Pro Max";
	if ([platform isEqualToString:@"iPhone15,4"]) return @"iPhone 15";
	if ([platform isEqualToString:@"iPhone15,5"]) return @"iPhone 15 Plus";
	if ([platform isEqualToString:@"iPhone16,1"]) return @"iPhone 15 Pro";
	if ([platform isEqualToString:@"iPhone16,2"]) return @"iPhone 15 Pro Max";
	
	if ([platform isEqualToString:@"iPod1,1"])   return @"iPod Touch 1G";
	if ([platform isEqualToString:@"iPod2,1"])   return @"iPod Touch 2G";
	if ([platform isEqualToString:@"iPod3,1"])   return @"iPod Touch 3G";
	if ([platform isEqualToString:@"iPod4,1"])   return @"iPod Touch 4G";
	if ([platform isEqualToString:@"iPod5,1"])   return @"iPod Touch 5G";
	if ([platform isEqualToString:@"iPod7,1"])   return @"iPod Touch 6G";
	if ([platform isEqualToString:@"iPod9,1"])   return @"iPod Touch 7G";

	if ([platform isEqualToString:@"iPad1,1"])   return @"iPad 1G (A1219/A1337)";
	if ([platform isEqualToString:@"iPad2,1"])   return @"iPad 2 (A1395)";
	if ([platform isEqualToString:@"iPad2,2"])   return @"iPad 2 (A1396)";
	if ([platform isEqualToString:@"iPad2,3"])   return @"iPad 2 (A1397)";
	if ([platform isEqualToString:@"iPad2,4"])   return @"iPad 2 (A1395+New Chip)";
	if ([platform isEqualToString:@"iPad2,5"])   return @"iPad Mini 1G (A1432)";
	if ([platform isEqualToString:@"iPad2,6"])   return @"iPad Mini 1G (A1454)";
	if ([platform isEqualToString:@"iPad2,7"])   return @"iPad Mini 1G (A1455)";
	if ([platform isEqualToString:@"iPad3,1"])   return @"iPad 3 (A1416)";
	if ([platform isEqualToString:@"iPad3,2"])   return @"iPad 3 (A1403)";
	if ([platform isEqualToString:@"iPad3,3"])   return @"iPad 3 (A1430)";
	if ([platform isEqualToString:@"iPad3,4"])   return @"iPad 4 (A1458)";
	if ([platform isEqualToString:@"iPad3,5"])   return @"iPad 4 (A1459)";
	if ([platform isEqualToString:@"iPad3,6"])   return @"iPad 4 (A1460)";
	if ([platform isEqualToString:@"iPad4,1"])   return @"iPad Air (A1474)";
	if ([platform isEqualToString:@"iPad4,2"])   return @"iPad Air (A1475)";
	if ([platform isEqualToString:@"iPad4,3"])   return @"iPad Air (A1476)";
	if ([platform isEqualToString:@"iPad4,4"])   return @"iPad Mini 2G (A1489)";
	if ([platform isEqualToString:@"iPad4,5"])   return @"iPad Mini 2G (A1490)";
	if ([platform isEqualToString:@"iPad4,6"])   return @"iPad Mini 2G (A1491)";
	if ([platform isEqualToString:@"iPad4,7"])   return @"iPad Mini 3";
	if ([platform isEqualToString:@"iPad4,8"])   return @"iPad Mini 3";
	if ([platform isEqualToString:@"iPad4,9"])   return @"iPad Mini 3";
	if ([platform isEqualToString:@"iPad5,1"])   return @"iPad Mini 4 (WiFi)";
	if ([platform isEqualToString:@"iPad5,2"])   return @"iPad Mini 4 (LTE)";
	if ([platform isEqualToString:@"iPad5,3"])   return @"iPad Air 2";
	if ([platform isEqualToString:@"iPad5,4"])   return @"iPad Air 2";
	if ([platform isEqualToString:@"iPad6,3"])   return @"iPad Pro 9.7";
	if ([platform isEqualToString:@"iPad6,4"])   return @"iPad Pro 9.7";
	if ([platform isEqualToString:@"iPad6,7"])   return @"iPad Pro 12.9";
	if ([platform isEqualToString:@"iPad6,8"])   return @"iPad Pro 12.9";
	if ([platform isEqualToString:@"iPad6,11"])   return @"iPad 5";
	if ([platform isEqualToString:@"iPad6,12"])   return @"iPad 5";
	if ([platform isEqualToString:@"iPad7,1"])   return @"iPad Pro 12.9 inch 2nd gen (WiFi)";
	if ([platform isEqualToString:@"iPad7,2"])   return @"iPad Pro 12.9 inch 2nd gen (Cellular)";
	if ([platform isEqualToString:@"iPad7,3"])   return @"iPad Pro 10.5 inch (WiFi)";
	if ([platform isEqualToString:@"iPad7,4"])   return @"iPad Pro 10.5 inch (Cellular)";
	if ([platform isEqualToString:@"iPad7,5"])   return @"iPad 6";
	if ([platform isEqualToString:@"iPad7,6"])   return @"iPad 6";
	if ([platform isEqualToString:@"iPad7,11"])   return @"iPad 7";
	if ([platform isEqualToString:@"iPad7,12"])   return @"iPad 7";
	if ([platform isEqualToString:@"iPad8,1"])   return @"iPad Pro 11-inch";
	if ([platform isEqualToString:@"iPad8,2"])   return @"iPad Pro 11-inch";
	if ([platform isEqualToString:@"iPad8,3"])   return @"iPad Pro 11-inch";
	if ([platform isEqualToString:@"iPad8,4"])   return @"iPad Pro 11-inch";
	if ([platform isEqualToString:@"iPad8,5"])   return @"iPad Pro 12.9-inch 3rd gen";
	if ([platform isEqualToString:@"iPad8,6"])   return @"iPad Pro 12.9-inch 3rd gen";
	if ([platform isEqualToString:@"iPad8,7"])   return @"iPad Pro 12.9-inch 3rd gen";
	if ([platform isEqualToString:@"iPad8,8"])   return @"iPad Pro 12.9-inch 3rd gen";
	if ([platform isEqualToString:@"iPad8,9"])   return @"iPad Pro 11-inch 2nd gen";
	if ([platform isEqualToString:@"iPad8,10"])   return @"iPad Pro 11-inch 2nd gen";
	if ([platform isEqualToString:@"iPad8,11"])   return @"iPad Pro 12.9-inch 4th gen";
	if ([platform isEqualToString:@"iPad8,12"])   return @"iPad Pro 12.9-inch 4th gen";
	if ([platform isEqualToString:@"iPad11,1"])   return @"iPad Mini 5";
	if ([platform isEqualToString:@"iPad11,2"])   return @"iPad Mini 5";
	if ([platform isEqualToString:@"iPad11,3"])   return @"iPad Air 3";
	if ([platform isEqualToString:@"iPad11,4"])   return @"iPad Air 3";
	if ([platform isEqualToString:@"iPad11,6"])   return @"iPad 8";
	if ([platform isEqualToString:@"iPad11,7"])   return @"iPad 8";
	if ([platform isEqualToString:@"iPad12,1"])   return @"iPad 9";
	if ([platform isEqualToString:@"iPad12,2"])   return @"iPad 9";
	if ([platform isEqualToString:@"iPad13,1"])   return @"iPad Air 4";
	if ([platform isEqualToString:@"iPad13,2"])   return @"iPad Air 4";
	if ([platform isEqualToString:@"iPad13,4"])   return @"iPad Pro 11-inch 3nd gen";
	if ([platform isEqualToString:@"iPad13,5"])   return @"iPad Pro 11-inch 3nd gen";
	if ([platform isEqualToString:@"iPad13,6"])   return @"iPad Pro 11-inch 3nd gen";
	if ([platform isEqualToString:@"iPad13,7"])   return @"iPad Pro 11-inch 3nd gen";
	if ([platform isEqualToString:@"iPad13,8"])   return @"iPad Pro 12.9-inch 5th gen";
	if ([platform isEqualToString:@"iPad13,9"])   return @"iPad Pro 12.9-inch 5th gen";
	if ([platform isEqualToString:@"iPad13,10"])   return @"iPad Pro 12.9-inch 5th gen";
	if ([platform isEqualToString:@"iPad13,11"])   return @"iPad Pro 12.9-inch 5th gen";
	if ([platform isEqualToString:@"iPad13,16"])   return @"iPad Air 5";
	if ([platform isEqualToString:@"iPad13,17"])   return @"iPad Air 5";
	if ([platform isEqualToString:@"iPad13,18"])   return @"iPad 10";
	if ([platform isEqualToString:@"iPad13,19"])   return @"iPad 10";
	if ([platform isEqualToString:@"iPad14,1"])   return @"iPad Mini 6";
	if ([platform isEqualToString:@"iPad14,2"])   return @"iPad Mini 6";
	if ([platform isEqualToString:@"iPad14,3"])   return @"iPad Pro 11-inch 4th gen";
	if ([platform isEqualToString:@"iPad14,4"])   return @"iPad Pro 11-inch 4th gen";
	if ([platform isEqualToString:@"iPad14,5"])   return @"iPad Pro 12.9-inch 6th gen";
	if ([platform isEqualToString:@"iPad14,6"])   return @"iPad Pro 12.9-inch 6th gen";

	if ([platform isEqualToString:@"AppleTV2,1"]) return @"Apple TV 2";
	if ([platform isEqualToString:@"AppleTV3,1"]) return @"Apple TV 3";
	if ([platform isEqualToString:@"AppleTV3,2"]) return @"Apple TV 3";
	if ([platform isEqualToString:@"AppleTV5,3"]) return @"Apple TV 4";
	
	if ([platform isEqualToString:@"i386"])	  return @"iPhone Simulator";
	if ([platform isEqualToString:@"x86_64"])	return @"iPhone Simulator";

	return platform;
}

- (NSString*)getCurrentDeviceID
{
	NSString *adid = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
	
	if(stringIsEmpty(adid))
	{
		return @"";
	}
	
	NSString *adidUp = [adid uppercaseString];	//大写
	
	if(stringIsEmpty(adidUp))
	{
		return @"";
	}
	
	return adidUp;
}

- (int)getPlatformID
{
	return mPlatformID;
}

- (int)CopyToClipboard:(NSString*)strValue
{
	@try {
		if(stringIsEmpty(strValue))
		{
			return 0;
		}
	
		UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
		[pasteboard setString:strValue];
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"CopyToClipboard-finally");
	}
	
	return 0;
}

- (int)JumpToAppStore:(NSString*)strValue
{
	if(stringIsEmpty(strValue))
	{
		return 0;
	}
	
	[[UIApplication sharedApplication] openURL:[NSURL URLWithString:strValue]];
	
	return 0;
}

- (void)initUtils
{
	@try {
		[self InitNotchFitMode];
		
		//UIApplication* app = [UIApplication sharedApplication];
		//if ([app currentUserNotificationSettings].types == UIUserNotificationTypeNone) {
		//   [app registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeBadge|UIUserNotificationTypeSound categories:nil]];
		//}
		
		if (@available(iOS 14.5, *)) {
			[ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
			}];
		}
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"initUtils-finally");
	}
}

- (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window
{
	return 0;
}

- (BOOL)openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
	return YES;
}

- (BOOL)openURL:(NSURL *)url options:(NSDictionary<NSString*, id> *)options
{
	return YES;
}

- (BOOL)handleOpenURL:(NSURL *)url
{
	return YES;
}

- (void)initSDK:(NSString*)language sdkLog:(bool)sdkLog
{
	@try {
		NSLog(@"initSDK");
		
		[self initSDKWithLanguage:language];
		
		isSDKLog = sdkLog ? TRUE : FALSE;
		
		NSString * strValue = [self PackageParam:@"1", nil];
		UnitySendMessage("ThirdPartyWrapper", "InitSDKResult", [strValue UTF8String]);
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"initSDK-finally");
	}
}

- (void)initVersion:(bool)bReviewPatentVersion reviewStoreVersion:(bool)bReviewStoreVersion
{
	@try {
		NSLog(@"initVersion");

		isReviewPatentVersion = bReviewPatentVersion ? TRUE : FALSE;
		isReviewStoreVersion = bReviewStoreVersion ? TRUE : FALSE;

		// 全球版 恒定不送审
		isReviewPatentVersion = FALSE;
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"initVersion-finally");
	}
}

- (void)loadConfig
{
	@try {
		NSLog(@"loading appconf");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"loadConfig-finally");
	}
}

- (void)changeLanguage:(NSString*)language
{
	NSLog(@"changeLanguage");
	
	if (stringIsEmpty(language)) {
		mstrLanguage = @"";
	} else {
		mstrLanguage = [[NSString alloc] initWithString:language];
	}
}

- (NSString*)getGameIDWithLanguage
{
	NSLog(@"getGameIDWithLanguage");
	
	if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"en"]) {
		return @"1085010303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"zh-Hans"]) {
		return @"1085190303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"zh-Hant"]) {
		return @"1085020303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"vi"]) {
		return @"1085150303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"es"]) {
		return @"1085380303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"pt"]) {
		return @"1085220303";
	} else if (mstrLanguage != nil && [mstrLanguage isEqualToString:@"id"]) {
		return @"1085110303";
	} else {
		return @"1085010303";
	}
}

- (void)initSDKWithLanguage:(NSString*)language
{
	@try {
		NSLog(@"initSDKWithLanguage");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"initSDKWithLanguage-finally");
	}
}

- (void)InitNotchFitMode
{
	@try {
		bool bNotchFit = false;
	
		if (@available(iOS 11.0, *)) {
			CGFloat height = [[UIApplication sharedApplication] delegate].window.safeAreaInsets.bottom;
			if (height > 0) {
				bNotchFit = true;
			}
		}
	
		if (bNotchFit) {
			NSLog(@"InitNotchFitMode NotchFit");
			NSString * strValue = [self PackageParam:@"1", @"60", @"400", nil];
			UnitySendMessage("ThirdPartyWrapper", "InitNotchFitModeResult", SDKMakeStringCopy([strValue UTF8String]));
		} else {
			NSLog(@"InitNotchFitMode Not NotchFit");
			NSString * strValue = [self PackageParam:@"0", @"0", @"0", nil];
			UnitySendMessage("ThirdPartyWrapper", "InitNotchFitModeResult", SDKMakeStringCopy([strValue UTF8String]));
		}
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"InitNotchFitMode-finally");
	}
}

- (void)loadUserInfoAfterLogin
{
	@try {
		
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"loadUserInfoAfterLogin-finally");
	}
}

- (void)login
{
	return;
}

- (void)logout
{
	return;
}

- (void)initPay
{
}

- (NSString*)getProductItems
{
	@try {
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"getProductItems-finally");
	}
	
	return @"";
}

- (NSString*)getAgreementURL:(int)nType
{
	return @"";
}

- (int)Pay:(NSString*)strProductID strJsonData:(NSString*)jsonData {
	@try {
		NSLog(@"[SDK_PAY] jsonData：%@", jsonData);
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"Pay-finally");
	}
	return 0;
}

-(void) RequestAgreementSignStatus
{
	@try {
		
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"RequestAgreementSignStatus-finally");
	}
}

-(void) SignAgreementSign
{
	@try {
		
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"SignAgreementSign-finally");
	}
}

-(void) RequestAssignedAgreements
{
	@try {
		strAgreementTermsOfServiceURL = @"";
		strPrivacyPolicyURL = @"";
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"RequestAssignedAgreements-finally");
	}
}

-(void) TranslateText:(NSString*)textid strText:(NSString*)text strDestLanguage:(NSString*)destLanguage
{
	@try {
		NSLog(@"TranslateText %@ %@ %@", textid, text, destLanguage);
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"TranslateText-finally");
	}
}

- (void)ExceptionLog:(NSString*)condition strInfo:(NSString*)info
{
	@try {
		//NSDictionary *userInfo = @{ @"condition":condition, @"info":info};
		//NSError *error = [NSError errorWithDomain:@"unity" code:-1001 userInfo:userInfo];
	}
	@catch (NSException *exception) {
		NSLog(@"ExceptionLog %@", exception);
	}
	@finally {
		NSLog(@"ExceptionLog-finally");
	}
}

- (void)EventLog:(NSString*)strEvent strIGGID:(NSString*)iggid strPlayerLv:(NSString*)playerLv strEMoney:(NSString*)emoney strMoney:(NSString*)money strCreateTime:(NSString*)createTime strIsWIFI:(NSString*)iswifi
{
    @try {
        NSDictionary *userInfo = @{ @"iggid":iggid, @"playerLv":playerLv, @"eMoney":emoney, @"money":money, @"createTime":createTime, @"isWifi":iswifi};
    }
    @catch (NSException *exception) {
        NSLog(@"EventLog %@", exception);
    }
    @finally {
        NSLog(@"EventLog-finally");
    }
}

-(void) RequestServiceURL
{
	@try {
		NSLog(@"RequestServiceURL");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"RequestServiceURL-finally");
	}
}

-(void) GetComplianceState
{
	@try {
		NSLog(@"GetComplianceState");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"GetComplianceState-finally");
	}
}

-(void) RealNameVerification
{
	@try {
		NSLog(@"RealNameVerification");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"RealNameVerification-finally");
	}
}

-(void) SyncComplianceState
{
	@try {
		// 同步防沉迷信息
		NSString* strInfo = [[NSString alloc] initWithFormat:@"%@,%@,%d,%@,%@,%f,%@,%@,%@",
				bEnablePreventWallow ? @"1" : @"0",
				bEnableRealNameVerification ? @"1" : @"0",
				nPreventWallowState,
				bRealNameVerificationState ? @"1" : @"0",
				bRealNameVerificationForceModel ? @"1" : @"0",
				fPreventWallowDuration,
				bInPreventWallowTime ? @"1" : @"0",
				stringIsEmpty(strPreventWallowTimeStart) ? @"" : strPreventWallowTimeStart,
				stringIsEmpty(strPreventWallowTimeEnd) ? @"" : strPreventWallowTimeEnd
			];

		NSLog(@"SyncComplianceState: %@", strInfo);

		NSString* strValue = [self PackageParam:@"1", strInfo, nil];
		UnitySendMessage("SDKInterface", "SyncComplianceState", SDKMakeStringCopy([strValue UTF8String]));
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"SyncComplianceState-finally");
	}
}

-(void) LoginWithDevice
{
	@try {
		NSLog(@"LoginWithDevice");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"LoginWithDevice-finally");
	}
}

-(void) ConfirmLoginWithDevice:(bool)value
{
	@try {
		NSLog(@"ConfirmLoginWithDevice");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"ConfirmLoginWithDevice-finally");
	}
}

-(void) LoginWithIGGPassport
{
	@try {
		NSLog(@"LoginWithIGGPassport");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"LoginWithIGGPassport-finally");
	}
}

-(void) ConfirmLoginWithIGGPassport:(bool)value
{
	@try {
		NSLog(@"ConfirmLoginWithIGGPassport %d", value);
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"ConfirmLoginWithIGGPassport-finally");
	}
}

- (void)BindWithIGGPassport
{
	@try {
		NSLog(@"BindWithIGGPassport");
	}
	@catch (NSException *exception) {
		NSLog(@"%@", exception);
	}
	@finally {
		NSLog(@"BindWithIGGPassport-finally");
	}
}

- (void)TrackEvent:(NSString*)strEvent strEventValues:(NSString*)eventValues
{
    @try {
        if (eventValues != nil) {
            UnitySendMessage("SDKInterface", "LogSDK", [eventValues UTF8String]);
        }
		NSData *data =  [eventValues dataUsingEncoding:NSASCIIStringEncoding];
		NSDictionary * userInfo = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingAllowFragments error:nil];
    }
    @catch (NSException *exception) {
        NSLog(@"EventTrack %@", exception);
    }
    @finally {
        NSLog(@"EventTrack-finally");
    }
}

- (void)ShareText:(NSString*)strTitle strContent:(NSString*)content
{
    NSString *shareTitle = strTitle;
    NSURL *shareURL = [NSURL URLWithString:content];
    NSArray *activityItems = @[shareTitle, shareURL];
    UIActivityViewController *activityVC = [[UIActivityViewController alloc]initWithActivityItems:activityItems applicationActivities:nil];
    //不出现在活动项目
    activityVC.excludedActivityTypes = @[UIActivityTypePrint, UIActivityTypeCopyToPasteboard,UIActivityTypeAssignToContact,UIActivityTypeSaveToCameraRoll];
    //这儿一定要做iPhone与iPad的判断，因为这儿只有iPhone可以present，iPad需pop，所以这儿actVC.popoverPresentationController.sourceView = self.view;
    //在iPad下必须有，不然iPad会crash
    UIViewController *vc = [UIApplication sharedApplication].keyWindow.rootViewController;
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        activityVC.popoverPresentationController.sourceView = vc.view;
        activityVC.popoverPresentationController.sourceRect = CGRectMake([UIScreen mainScreen].bounds.size.width/2, [UIScreen mainScreen].bounds.size.height, 0, 0);
        [vc presentViewController:activityVC animated:YES completion:nil];
    }else{
        [vc presentViewController:activityVC animated:YES completion:nil];
    }
    // 分享之后的回调
    activityVC.completionWithItemsHandler = ^(UIActivityType  _Nullable activityType, BOOL completed, NSArray * _Nullable returnedItems, NSError * _Nullable activityError) {
        if (completed) {
            //分享 成功
           NSLog(@"ShareText completed");
        } else  {
            //分享 取消
           NSLog(@"ShareText cancled");
        }
    };
}

- (int)OpenUrlScheme:(NSString*)text
{
	if(stringIsEmpty(text))
	{
		return 0;
	}
	
	if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:text]]) {
		[[UIApplication sharedApplication] openURL:[NSURL URLWithString:text]];
		return 1;
	} else {
		return 0;
	}
}

@end

const char* SDKUtilsGetDeviceName()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getCurrentDevice] UTF8String]);
}

const char* SDKUtilsGetDeviceID()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getCurrentDeviceID] UTF8String]);
}

const char* SDKUtilsGetChannelName()
{
	NSString* value = @"ios";
	return SDKMakeStringCopy([value UTF8String]);
}

const char* SDKUtilsGetSystemLanguage()
{
	NSArray *languages = [NSLocale preferredLanguages];
	NSString *currentLanguage = [languages objectAtIndex:0];
	return SDKMakeStringCopy([currentLanguage UTF8String]);
}

const char* SDKUtilsGetSystemVersion()
{
	NSString *currentVersion = [[UIDevice currentDevice] systemVersion];
	return SDKMakeStringCopy([currentVersion UTF8String]);
}

const char* SDKUtilsGetPlatformParam1()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getPlatformParam1] UTF8String]);
}

const char* SDKUtilsGetPlatformParam2()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getPlatformParam2] UTF8String]);
}

const char* SDKUtilsGetPlatformParam3()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getPlatformParam3] UTF8String]);
}

const char* SDKUtilsGetProductItems()
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getProductItems] UTF8String]);
}

const char* SDKUtilsGetAgreementURL(int nType)
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
	return SDKMakeStringCopy([[sdkUtils getAgreementURL:nType] UTF8String]);
}
