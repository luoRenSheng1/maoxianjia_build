import './getPlugin';
import { formatJsonStr } from './utils';
console.log("import getPlugin")

function getDefaultData(canvas, conf) {
    const config = formatJsonStr(conf);
    if (typeof config.x === 'undefined') {
        config.x = 0;
    }
    if (typeof config.y === 'undefined') {
        config.y = 0;
    }
    if (typeof config.width === 'undefined' || config.width === 0) {
        config.width = canvas.width;
    }
    if (typeof config.height === 'undefined' || config.height === 0) {
        config.height = canvas.height;
    }
    if (typeof config.destWidth === 'undefined' || config.destWidth === 0) {
        config.destWidth = canvas.width;
    }
    if (typeof config.destHeight === 'undefined' || config.destHeight === 0) {
        config.destHeight = canvas.height;
    }
    return config;
}

function sendAction(callbackId, status, result) {
    console.log('sendAction', callbackId, status, result);
    const resStr = JSON.stringify({
        callbackId: callbackId,
        errCode: status,
        errMsg: JSON.stringify(result),
    });
    GameGlobal.Module.SendMessage(className, methodName, resStr);
}

let gameClub;
let className = 'ThirdPartyWrapper';
let methodName = 'OnShareImageToGameCenterCallback';
var gameCenterIO = {
    // 分享CDN图片到游戏圈
    shareImageToGameCenter: function (conf, callbackId) {
        const config = formatJsonStr(conf);
        const url = config.url;
        const title = config.title;
        const content = config.content;
        
        console.log("shareImageToGameCenter", config, url, title, content, callbackId)
        if (GameGlobal.miniGameCommon && GameGlobal.miniGameCommon.canIUse('shareImageToGameCenter')) {
            if (!gameClub) {
                gameClub = GameGlobal.miniGameCommon.createGameClub();
            }
        } else {
            console.log("功能未开启")
        }
        if (gameClub) {
            wx.showLoading({
                title: '正在加载...',
            });
            wx.downloadFile({
                url, // 此处替换为开发者自己的cdn图片地址，要在MP中配置域名
                success: (res) => {
                    wx.hideLoading();
                    if (res.statusCode === 200) {
                        gameClub
                            .shareImageToGameCenter({
                                path: res.tempFilePath,
                                title: title,
                                content: content,
                            })
                            .then((res) => {
                                // 分享成功，自动跳转到游戏圈
                                sendAction(callbackId, 1, res);
                            })
                            .catch((err) => {
                                // 分享失败
                                wx.showToast({
                                    icon: 'none',
                                    title: '分享失败，请稍后再试',
                                });
                                sendAction(callbackId, 0, err);
                            });
                    }
                },
                fail: (err) => {
                    wx.hideLoading();
                    console.log('下载cdn图片出错', err);
                    sendAction(callbackId, 0, err);
                },
            });
        } else {
            // 插件尚未初始化
            console.error('插件尚未初始化');
            sendAction(callbackId, -1, 'not support');
        }
    },
    // 分享canvas导出到游戏圈
    // conf: { x: number, y: number, width: number, height: number, destWidth: number, destHeight: number }
    shareCanvasToGameCenter: function (conf, callbackId) {
        const config = formatJsonStr(conf);
        const title = config.title;
        const content = config.content;
        
        console.log("shareCanvasToGameCenter", config, title, content, callbackId)
        if (GameGlobal.miniGameCommon && GameGlobal.miniGameCommon.canIUse('shareImageToGameCenter')) {
            if (!gameClub) {
                gameClub = GameGlobal.miniGameCommon.createGameClub();
            }
        } else {
            console.log("功能未开启")
        }
        if (gameClub) {
            canvas.toTempFilePath({
                ...getDefaultData(canvas, config),
                success: (res) => {
                    gameClub
                        .shareImageToGameCenter({
                            path: res.tempFilePath,
                            title: title,
                            content: content,
                        })
                        .then((res) => {
                            // 分享成功，自动跳转到游戏圈
                            sendAction(callbackId, 1, res);
                        })
                        .catch((err) => {
                            // 分享失败
                            wx.showToast({
                                icon: 'none',
                                title: '分享失败，请稍后再试',
                            });
                            sendAction(callbackId, 0, err);
                        });
                },
                fail: (err) => {
                    // 导出失败
                    wx.showToast({
                        icon: 'none',
                        title: '截图失败，请稍后再试',
                    });
                    sendAction(callbackId, 0, err);
                },
            });
        } else {
            // 插件尚未初始化
            console.error('插件尚未初始化');
            sendAction(callbackId, -1, 'not support');
        }
    }
};

module.exports = gameCenterIO;
