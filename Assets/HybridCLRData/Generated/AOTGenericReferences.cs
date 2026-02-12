using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Google.Protobuf.dll",
		"Google.ProtocolBuffers.dll",
		"LitJson.dll",
		"System.Core.dll",
		"System.dll",
		"ThirdParty.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.AssetBundleModule.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Google.Protobuf.Collections.MapField.<>c<int,object>
	// Google.Protobuf.Collections.MapField.<>c<object,object>
	// Google.Protobuf.Collections.MapField.<>c__DisplayClass7_0<int,object>
	// Google.Protobuf.Collections.MapField.<>c__DisplayClass7_0<object,object>
	// Google.Protobuf.Collections.MapField.Codec.MessageAdapter<int,object>
	// Google.Protobuf.Collections.MapField.Codec.MessageAdapter<object,object>
	// Google.Protobuf.Collections.MapField.Codec<int,object>
	// Google.Protobuf.Collections.MapField.Codec<object,object>
	// Google.Protobuf.Collections.MapField.DictionaryEnumerator<int,object>
	// Google.Protobuf.Collections.MapField.DictionaryEnumerator<object,object>
	// Google.Protobuf.Collections.MapField.MapView<int,object,int>
	// Google.Protobuf.Collections.MapField.MapView<int,object,object>
	// Google.Protobuf.Collections.MapField.MapView<object,object,object>
	// Google.Protobuf.Collections.MapField<int,object>
	// Google.Protobuf.Collections.MapField<object,object>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__22<int>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__22<object>
	// Google.Protobuf.Collections.RepeatedField<int>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.FieldCodec.<>c__16<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass16_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass27_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass27_0<object>
	// Google.Protobuf.FieldCodec<int>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.IDeepCloneable<int>
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.IMessage<object>
	// Google.Protobuf.MessageParser.<>c__DisplayClass2_0<object>
	// Google.Protobuf.MessageParser<object>
	// Google.ProtocolBuffers.AbstractBuilder<object,object>
	// Google.ProtocolBuffers.AbstractBuilderLite.LimitedInputStream<object,object>
	// Google.ProtocolBuffers.AbstractBuilderLite<object,object>
	// Google.ProtocolBuffers.AbstractMessage<object,object>
	// Google.ProtocolBuffers.AbstractMessageLite<object,object>
	// Google.ProtocolBuffers.Action<object,object>
	// Google.ProtocolBuffers.Collections.Lists<byte>
	// Google.ProtocolBuffers.Collections.Lists<double>
	// Google.ProtocolBuffers.Collections.Lists<float>
	// Google.ProtocolBuffers.Collections.Lists<int>
	// Google.ProtocolBuffers.Collections.Lists<long>
	// Google.ProtocolBuffers.Collections.Lists<object>
	// Google.ProtocolBuffers.Collections.Lists<uint>
	// Google.ProtocolBuffers.Collections.Lists<ulong>
	// Google.ProtocolBuffers.Collections.PopsicleList<byte>
	// Google.ProtocolBuffers.Collections.PopsicleList<double>
	// Google.ProtocolBuffers.Collections.PopsicleList<float>
	// Google.ProtocolBuffers.Collections.PopsicleList<int>
	// Google.ProtocolBuffers.Collections.PopsicleList<long>
	// Google.ProtocolBuffers.Collections.PopsicleList<object>
	// Google.ProtocolBuffers.Collections.PopsicleList<uint>
	// Google.ProtocolBuffers.Collections.PopsicleList<ulong>
	// Google.ProtocolBuffers.DescriptorProtos.IDescriptorProto<object>
	// Google.ProtocolBuffers.Descriptors.DescriptorBase<object,object>
	// Google.ProtocolBuffers.Descriptors.IndexedDescriptorBase<object,object>
	// Google.ProtocolBuffers.FieldAccess.FieldAccessorTable<object,object>
	// Google.ProtocolBuffers.FieldAccess.IFieldAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.RepeatedEnumAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.RepeatedMessageAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.RepeatedPrimitiveAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.SingleEnumAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.SingleMessageAccessor<object,object>
	// Google.ProtocolBuffers.FieldAccess.SinglePrimitiveAccessor<object,object>
	// Google.ProtocolBuffers.Func<object,byte>
	// Google.ProtocolBuffers.Func<object,int>
	// Google.ProtocolBuffers.Func<object,object>
	// Google.ProtocolBuffers.Func<object>
	// Google.ProtocolBuffers.GeneratedBuilder<object,object>
	// Google.ProtocolBuffers.GeneratedMessage<object,object>
	// System.Action<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Action<BestHTTP.Core.ConnectionEventInfo>
	// System.Action<BestHTTP.Core.PluginEventInfo>
	// System.Action<BestHTTP.Core.ProtocolEventInfo>
	// System.Action<BestHTTP.Core.RequestEventInfo>
	// System.Action<BestHTTP.Extensions.TimerData>
	// System.Action<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Action<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Action<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Action<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Action<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Action<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Action<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Action<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Action<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Action<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Action<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Action<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Action<BestHTTP.SignalRCore.Messages.Message>
	// System.Action<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Action<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Action<BestHTTP.Timings.TimingEvent>
	// System.Action<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Action<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Action<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Action<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Action<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Action<System.ValueTuple<object,object>>
	// System.Action<UnityEngine.Quaternion>
	// System.Action<UnityEngine.Vector2>
	// System.Action<UnityEngine.Vector2Int>
	// System.Action<UnityEngine.Vector3>
	// System.Action<byte,object,object,byte>
	// System.Action<byte,object>
	// System.Action<byte>
	// System.Action<double>
	// System.Action<float>
	// System.Action<int,byte,int>
	// System.Action<int,int>
	// System.Action<int,object>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Action<object,BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Action<object,double>
	// System.Action<object,int>
	// System.Action<object,object,int>
	// System.Action<object,object,object,object,object>
	// System.Action<object,object,object,object>
	// System.Action<object,object,object>
	// System.Action<object,object>
	// System.Action<object,ushort,object>
	// System.Action<object,ushort,uint,uint>
	// System.Action<object>
	// System.Action<short>
	// System.Action<uint>
	// System.Action<ulong>
	// System.Action<ushort>
	// System.ByReference<ushort>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.Logger.LogJob>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.Logger.LogJob>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.Logger.LogJob>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Concurrent.ConcurrentQueue<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.ArraySortHelper<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Quaternion>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2Int>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<double>
	// System.Collections.Generic.ArraySortHelper<float>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object,object>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.ArraySortHelper<short>
	// System.Collections.Generic.ArraySortHelper<uint>
	// System.Collections.Generic.ArraySortHelper<ulong>
	// System.Collections.Generic.ArraySortHelper<ushort>
	// System.Collections.Generic.Comparer<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.Comparer<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.Comparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Comparer<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.Comparer<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.Comparer<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.Comparer<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.Comparer<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.Comparer<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.Comparer<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.Comparer<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.Comparer<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.Comparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.Comparer<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.Comparer<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.Comparer<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.Comparer<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.Comparer<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.Comparer<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<UnityEngine.Quaternion>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<double>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<short>
	// System.Collections.Generic.Comparer<uint>
	// System.Collections.Generic.Comparer<ulong>
	// System.Collections.Generic.Comparer<ushort>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.ComparisonComparer<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ComparisonComparer<byte>
	// System.Collections.Generic.ComparisonComparer<double>
	// System.Collections.Generic.ComparisonComparer<float>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<long>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.ComparisonComparer<short>
	// System.Collections.Generic.ComparisonComparer<uint>
	// System.Collections.Generic.ComparisonComparer<ulong>
	// System.Collections.Generic.ComparisonComparer<ushort>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.Enumerator<int,double>
	// System.Collections.Generic.Dictionary.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,ushort>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,double>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,int>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<int,double>
	// System.Collections.Generic.Dictionary.KeyCollection<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,double>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,int>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<int,double>
	// System.Collections.Generic.Dictionary.ValueCollection<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,int>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector2,byte>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector2,object>
	// System.Collections.Generic.Dictionary<int,byte>
	// System.Collections.Generic.Dictionary<int,double>
	// System.Collections.Generic.Dictionary<int,float>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary<object,SoundManager.AudioInfo>
	// System.Collections.Generic.Dictionary<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<uint,ushort>
	// System.Collections.Generic.Dictionary<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.Dictionary<ulong,object>
	// System.Collections.Generic.Dictionary<ushort,int>
	// System.Collections.Generic.EqualityComparer<BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.EqualityComparer<BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.EqualityComparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.EqualityComparer<BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.EqualityComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.EqualityComparer<SoundManager.AudioInfo>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<double>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.EqualityComparer<ulong>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet.Enumerator<ulong>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet<ulong>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.HashSetEqualityComparer<ulong>
	// System.Collections.Generic.ICollection<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.ICollection<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Generic.ICollection<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Generic.ICollection<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Generic.ICollection<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Generic.ICollection<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.ICollection<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.ICollection<BestHTTP.Logger.LogJob>
	// System.Collections.Generic.ICollection<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.ICollection<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.ICollection<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.ICollection<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.ICollection<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.ICollection<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.ICollection<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.ICollection<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.ICollection<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.ICollection<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.ICollection<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.ICollection<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.ICollection<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.ICollection<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.ICollection<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Generic.ICollection<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,double>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ArrayMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ObjectMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.PropertyMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,SoundManager.AudioInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,ushort>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,BestHTTP.SignalR.Messages.ClientMessage>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.ICollection<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<UnityEngine.Quaternion>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2Int>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<double>
	// System.Collections.Generic.ICollection<float>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<short>
	// System.Collections.Generic.ICollection<uint>
	// System.Collections.Generic.ICollection<ulong>
	// System.Collections.Generic.ICollection<ushort>
	// System.Collections.Generic.IComparer<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.IComparer<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.IComparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.IComparer<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.IComparer<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.IComparer<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.IComparer<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.IComparer<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.IComparer<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.IComparer<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.IComparer<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.IComparer<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.IComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IComparer<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.IComparer<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.IComparer<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.IComparer<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.IComparer<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.IComparer<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<double>
	// System.Collections.Generic.IComparer<float>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<short>
	// System.Collections.Generic.IComparer<uint>
	// System.Collections.Generic.IComparer<ulong>
	// System.Collections.Generic.IComparer<ushort>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IDictionary<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IDictionary<object,int>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IDictionary<ushort,int>
	// System.Collections.Generic.IEnumerable<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.IEnumerable<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Generic.IEnumerable<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Generic.IEnumerable<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Generic.IEnumerable<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Generic.IEnumerable<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.IEnumerable<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerable<BestHTTP.Logger.LogJob>
	// System.Collections.Generic.IEnumerable<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.IEnumerable<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.IEnumerable<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.IEnumerable<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.IEnumerable<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.IEnumerable<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.IEnumerable<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.IEnumerable<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.IEnumerable<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.IEnumerable<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IEnumerable<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.IEnumerable<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.IEnumerable<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.IEnumerable<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.IEnumerable<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Generic.IEnumerable<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,double>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,BestHTTP.SignalRCore.InvocationDefinition>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ArrayMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ObjectMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.PropertyMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,SoundManager.AudioInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,ushort>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,BestHTTP.SignalR.Messages.ClientMessage>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.Quaternion>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<double>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<short>
	// System.Collections.Generic.IEnumerable<uint>
	// System.Collections.Generic.IEnumerable<ulong>
	// System.Collections.Generic.IEnumerable<ushort>
	// System.Collections.Generic.IEnumerator<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.IEnumerator<BestHTTP.Core.ConnectionEventInfo>
	// System.Collections.Generic.IEnumerator<BestHTTP.Core.PluginEventInfo>
	// System.Collections.Generic.IEnumerator<BestHTTP.Core.ProtocolEventInfo>
	// System.Collections.Generic.IEnumerator<BestHTTP.Core.RequestEventInfo>
	// System.Collections.Generic.IEnumerator<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.IEnumerator<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerator<BestHTTP.Logger.LogJob>
	// System.Collections.Generic.IEnumerator<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.IEnumerator<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.IEnumerator<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.IEnumerator<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.IEnumerator<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.IEnumerator<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.IEnumerator<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.IEnumerator<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.IEnumerator<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.IEnumerator<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IEnumerator<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.IEnumerator<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.IEnumerator<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.IEnumerator<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.IEnumerator<BestHTTP.WebSocket.Frames.WebSocketFrame>
	// System.Collections.Generic.IEnumerator<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,double>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,BestHTTP.SignalRCore.InvocationDefinition>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ArrayMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ObjectMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.PropertyMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,SoundManager.AudioInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,ushort>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,BestHTTP.SignalR.Messages.ClientMessage>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.Quaternion>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<double>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<short>
	// System.Collections.Generic.IEnumerator<uint>
	// System.Collections.Generic.IEnumerator<ulong>
	// System.Collections.Generic.IEnumerator<ushort>
	// System.Collections.Generic.IEqualityComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IEqualityComparer<ulong>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.IList<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.IList<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.IList<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.IList<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.IList<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.IList<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.IList<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.IList<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.IList<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.IList<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.IList<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.IList<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.IList<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.IList<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.IList<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.IList<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.IList<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.IList<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<UnityEngine.Quaternion>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<UnityEngine.Vector2Int>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<double>
	// System.Collections.Generic.IList<float>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IList<short>
	// System.Collections.Generic.IList<uint>
	// System.Collections.Generic.IList<ulong>
	// System.Collections.Generic.IList<ushort>
	// System.Collections.Generic.ISet<object>
	// System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,byte>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,object>
	// System.Collections.Generic.KeyValuePair<int,byte>
	// System.Collections.Generic.KeyValuePair<int,double>
	// System.Collections.Generic.KeyValuePair<int,float>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.KeyValuePair<object,BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.KeyValuePair<object,SoundManager.AudioInfo>
	// System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.KeyValuePair<uint,uint>
	// System.Collections.Generic.KeyValuePair<uint,ushort>
	// System.Collections.Generic.KeyValuePair<ulong,BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.KeyValuePair<ulong,object>
	// System.Collections.Generic.KeyValuePair<ushort,int>
	// System.Collections.Generic.KeyValuePair<ushort,uint>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.List.Enumerator<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.List.Enumerator<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.List.Enumerator<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.List.Enumerator<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.List.Enumerator<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.List.Enumerator<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.List.Enumerator<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.List.Enumerator<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.List.Enumerator<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.List.Enumerator<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.List.Enumerator<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Quaternion>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<double>
	// System.Collections.Generic.List.Enumerator<float>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<short>
	// System.Collections.Generic.List.Enumerator<uint>
	// System.Collections.Generic.List.Enumerator<ulong>
	// System.Collections.Generic.List.Enumerator<ushort>
	// System.Collections.Generic.List<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.List<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.List<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.List<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.List<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.List<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.List<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.List<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.List<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.List<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.List<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.List<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.List<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.List<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.List<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.List<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.List<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.List<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.List<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<UnityEngine.Quaternion>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<UnityEngine.Vector2Int>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<double>
	// System.Collections.Generic.List<float>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<short>
	// System.Collections.Generic.List<uint>
	// System.Collections.Generic.List<ulong>
	// System.Collections.Generic.List<ushort>
	// System.Collections.Generic.ObjectComparer<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.ObjectComparer<BestHTTP.Extensions.TimerData>
	// System.Collections.Generic.ObjectComparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.ObjectComparer<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.Generic.ObjectComparer<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.Generic.ObjectComparer<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.Generic.ObjectComparer<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.Generic.ObjectComparer<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.Generic.ObjectComparer<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.Generic.ObjectComparer<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.Generic.ObjectComparer<BestHTTP.Timings.TimingEvent>
	// System.Collections.Generic.ObjectComparer<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<double>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<short>
	// System.Collections.Generic.ObjectComparer<uint>
	// System.Collections.Generic.ObjectComparer<ulong>
	// System.Collections.Generic.ObjectComparer<ushort>
	// System.Collections.Generic.ObjectEqualityComparer<BestHTTP.JSON.LitJson.ArrayMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<BestHTTP.JSON.LitJson.ObjectMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<BestHTTP.SignalR.Messages.ClientMessage>
	// System.Collections.Generic.ObjectEqualityComparer<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.Generic.ObjectEqualityComparer<SoundManager.AudioInfo>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<double>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ulong>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue.Enumerator<ulong>
	// System.Collections.Generic.Queue<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.Generic.Queue<int>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Queue<ulong>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<int,byte>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<int,byte>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,byte>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<int,byte>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<int,byte>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<int,byte>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<int,byte>
	// System.Collections.Generic.SortedDictionary.KeyCollection<int,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<int,byte>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<int,byte>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<int,byte>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,byte>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,byte>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary<int,byte>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedList.Enumerator<object,object>
	// System.Collections.Generic.SortedList.KeyList<object,object>
	// System.Collections.Generic.SortedList.SortedListKeyEnumerator<object,object>
	// System.Collections.Generic.SortedList.SortedListValueEnumerator<object,object>
	// System.Collections.Generic.SortedList.ValueList<object,object>
	// System.Collections.Generic.SortedList<object,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Stack.Enumerator<int>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<int>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.Extensions.TimerData>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SignalRCore.Messages.Message>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.Timings.TimingEvent>
	// System.Collections.ObjectModel.ReadOnlyCollection<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Quaternion>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2Int>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<double>
	// System.Collections.ObjectModel.ReadOnlyCollection<float>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<short>
	// System.Collections.ObjectModel.ReadOnlyCollection<uint>
	// System.Collections.ObjectModel.ReadOnlyCollection<ulong>
	// System.Collections.ObjectModel.ReadOnlyCollection<ushort>
	// System.Comparison<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Comparison<BestHTTP.Extensions.TimerData>
	// System.Comparison<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Comparison<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Comparison<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Comparison<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Comparison<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Comparison<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Comparison<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Comparison<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Comparison<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Comparison<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Comparison<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Comparison<BestHTTP.SignalRCore.Messages.Message>
	// System.Comparison<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Comparison<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Comparison<BestHTTP.Timings.TimingEvent>
	// System.Comparison<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Comparison<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Comparison<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<UnityEngine.Quaternion>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<UnityEngine.Vector2Int>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<byte>
	// System.Comparison<double>
	// System.Comparison<float>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Comparison<short>
	// System.Comparison<uint>
	// System.Comparison<ulong>
	// System.Comparison<ushort>
	// System.Converter<int,object>
	// System.Converter<object,int>
	// System.Dynamic.Utils.CacheDict.Entry<object,object>
	// System.Dynamic.Utils.CacheDict<object,object>
	// System.EventHandler<object>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,System.Collections.DictionaryEntry>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,System.Collections.DictionaryEntry>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Func<System.DateTime,object,byte>
	// System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,byte>
	// System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>
	// System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>
	// System.Func<byte>
	// System.Func<int,byte>
	// System.Func<int,int>
	// System.Func<long,BestHTTP.SignalRCore.InvocationDefinition,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Func<long,BestHTTP.SignalRCore.InvocationDefinition>
	// System.Func<object,BestHTTP.SignalRCore.Messages.Message,byte>
	// System.Func<object,System.ValueTuple<int,int>>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,byte,object,object>
	// System.Func<object,object,object,int,byte>
	// System.Func<object,object,object,object,object>
	// System.Func<object,object,object,object>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object,ulong>
	// System.Func<object>
	// System.Func<ushort,byte>
	// System.IComparable<object>
	// System.IEquatable<BestHTTP.Timings.TimingEvent>
	// System.IEquatable<object>
	// System.Linq.Buffer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Buffer<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Buffer<int>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<CastIterator>d__99<object>
	// System.Linq.Enumerable.<DistinctIterator>d__68<int>
	// System.Linq.Enumerable.<DistinctIterator>d__68<object>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.Iterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<int,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<int,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectListIterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>
	// System.Linq.Enumerable.WhereSelectListIterator<int,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.EnumerableSorter<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>
	// System.Linq.EnumerableSorter<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.EnumerableSorter<int,int>
	// System.Linq.EnumerableSorter<int>
	// System.Linq.EnumerableSorter<object,byte>
	// System.Linq.EnumerableSorter<object,int>
	// System.Linq.EnumerableSorter<object>
	// System.Linq.Expressions.Expression<object>
	// System.Linq.GroupedEnumerable<object,int,object>
	// System.Linq.IGrouping<int,object>
	// System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.IOrderedEnumerable<int>
	// System.Linq.IOrderedEnumerable<object>
	// System.Linq.IdentityFunction.<>c<object>
	// System.Linq.IdentityFunction<object>
	// System.Linq.Lookup.<GetEnumerator>d__12<int,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<int,object>
	// System.Linq.Lookup.Grouping<int,object>
	// System.Linq.Lookup<int,object>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<int>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
	// System.Linq.OrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>
	// System.Linq.OrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Linq.OrderedEnumerable<int,int>
	// System.Linq.OrderedEnumerable<int>
	// System.Linq.OrderedEnumerable<object,byte>
	// System.Linq.OrderedEnumerable<object,int>
	// System.Linq.OrderedEnumerable<object>
	// System.Linq.OrderedParallelQuery<int>
	// System.Linq.OrderedParallelQuery<object>
	// System.Linq.OrderedParallelQuery<ulong>
	// System.Linq.Parallel.ArrayMergeHelper<int>
	// System.Linq.Parallel.ArrayMergeHelper<object>
	// System.Linq.Parallel.ArrayMergeHelper<ulong>
	// System.Linq.Parallel.AsynchronousChannel<int>
	// System.Linq.Parallel.AsynchronousChannel<object>
	// System.Linq.Parallel.AsynchronousChannel<ulong>
	// System.Linq.Parallel.IMergeHelper<int>
	// System.Linq.Parallel.IMergeHelper<object>
	// System.Linq.Parallel.IMergeHelper<ulong>
	// System.Linq.Parallel.IPartitionedStreamRecipient<int>
	// System.Linq.Parallel.IPartitionedStreamRecipient<object>
	// System.Linq.Parallel.IPartitionedStreamRecipient<ulong>
	// System.Linq.Parallel.ListQueryResults<int>
	// System.Linq.Parallel.ListQueryResults<object>
	// System.Linq.Parallel.ListQueryResults<ulong>
	// System.Linq.Parallel.MergeExecutor<int>
	// System.Linq.Parallel.MergeExecutor<object>
	// System.Linq.Parallel.MergeExecutor<ulong>
	// System.Linq.Parallel.ParallelEnumerableWrapper<int>
	// System.Linq.Parallel.ParallelEnumerableWrapper<object>
	// System.Linq.Parallel.ParallelEnumerableWrapper<ulong>
	// System.Linq.Parallel.PartitionedStreamMerger<int>
	// System.Linq.Parallel.PartitionedStreamMerger<object>
	// System.Linq.Parallel.PartitionedStreamMerger<ulong>
	// System.Linq.Parallel.QueryExecutionOption<int>
	// System.Linq.Parallel.QueryOpeningEnumerator<int>
	// System.Linq.Parallel.QueryOpeningEnumerator<object>
	// System.Linq.Parallel.QueryOpeningEnumerator<ulong>
	// System.Linq.Parallel.QueryOperator<int>
	// System.Linq.Parallel.QueryOperator<object>
	// System.Linq.Parallel.QueryOperator<ulong>
	// System.Linq.Parallel.QueryResults.<System-Collections-Generic-IEnumerable<T>-GetEnumerator>d__21<int>
	// System.Linq.Parallel.QueryResults.<System-Collections-Generic-IEnumerable<T>-GetEnumerator>d__21<object>
	// System.Linq.Parallel.QueryResults.<System-Collections-Generic-IEnumerable<T>-GetEnumerator>d__21<ulong>
	// System.Linq.Parallel.QueryResults<int>
	// System.Linq.Parallel.QueryResults<object>
	// System.Linq.Parallel.QueryResults<ulong>
	// System.Linq.Parallel.ScanQueryOperator.ScanEnumerableQueryOperatorResults<int>
	// System.Linq.Parallel.ScanQueryOperator.ScanEnumerableQueryOperatorResults<object>
	// System.Linq.Parallel.ScanQueryOperator.ScanEnumerableQueryOperatorResults<ulong>
	// System.Linq.Parallel.ScanQueryOperator<int>
	// System.Linq.Parallel.ScanQueryOperator<object>
	// System.Linq.Parallel.ScanQueryOperator<ulong>
	// System.Linq.Parallel.SelectQueryOperator.SelectQueryOperatorResults<object,int>
	// System.Linq.Parallel.SelectQueryOperator.SelectQueryOperatorResults<object,ulong>
	// System.Linq.Parallel.SelectQueryOperator<object,int>
	// System.Linq.Parallel.SelectQueryOperator<object,ulong>
	// System.Linq.Parallel.Shared<byte>
	// System.Linq.Parallel.SynchronousChannel<int>
	// System.Linq.Parallel.SynchronousChannel<object>
	// System.Linq.Parallel.SynchronousChannel<ulong>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults.ChildResultsRecipient<object,int>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults.ChildResultsRecipient<object,object>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults.ChildResultsRecipient<object,ulong>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults<object,int>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults<object,object>
	// System.Linq.Parallel.UnaryQueryOperator.UnaryQueryOperatorResults<object,ulong>
	// System.Linq.Parallel.UnaryQueryOperator<object,int>
	// System.Linq.Parallel.UnaryQueryOperator<object,object>
	// System.Linq.Parallel.UnaryQueryOperator<object,ulong>
	// System.Linq.Parallel.WhereQueryOperator<object>
	// System.Linq.ParallelQuery<int>
	// System.Linq.ParallelQuery<object>
	// System.Linq.ParallelQuery<ulong>
	// System.Linq.Set<int>
	// System.Linq.Set<object>
	// System.Nullable<System.DateTime>
	// System.Nullable<System.TimeSpan>
	// System.Nullable<UnityEngine.Vector2>
	// System.Nullable<byte>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Nullable<uint>
	// System.Nullable<ushort>
	// System.Predicate<BestHTTP.Connections.HTTP2.HTTP2FrameHeaderAndPayload>
	// System.Predicate<BestHTTP.Extensions.TimerData>
	// System.Predicate<BestHTTP.JSON.LitJson.PropertyMetadata>
	// System.Predicate<BestHTTP.Logger.LoggingContext.LoggingContextField>
	// System.Predicate<BestHTTP.PlatformSupport.Memory.BufferDesc>
	// System.Predicate<BestHTTP.PlatformSupport.Memory.BufferPool.BufferStats>
	// System.Predicate<BestHTTP.PlatformSupport.Memory.BufferSegment>
	// System.Predicate<BestHTTP.PlatformSupport.Memory.BufferStore>
	// System.Predicate<BestHTTP.PlatformSupport.Text.StringBuilderPool.BuilderShelf>
	// System.Predicate<BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm.GcmUtilities.FieldElement>
	// System.Predicate<BestHTTP.SignalRCore.CallbackDescriptor>
	// System.Predicate<BestHTTP.SignalRCore.FunctionCallbackDescriptor>
	// System.Predicate<BestHTTP.SignalRCore.InvocationDefinition>
	// System.Predicate<BestHTTP.SignalRCore.Messages.Message>
	// System.Predicate<BestHTTP.SocketIO3.Events.CallbackDescriptor>
	// System.Predicate<BestHTTP.SocketIO3.OutgoingPacket>
	// System.Predicate<BestHTTP.Timings.TimingEvent>
	// System.Predicate<BestHTTP.WebSocket.Frames.WebSocketFrameReader>
	// System.Predicate<System.Collections.Generic.KeyValuePair<BestHTTP.PlatformSupport.Memory.BufferSegment,BestHTTP.PlatformSupport.Memory.BufferSegment>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<ushort,uint>>
	// System.Predicate<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<UnityEngine.Quaternion>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<UnityEngine.Vector2Int>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<byte>
	// System.Predicate<double>
	// System.Predicate<float>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Predicate<short>
	// System.Predicate<uint>
	// System.Predicate<ulong>
	// System.Predicate<ushort>
	// System.ReadOnlySpan<ushort>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.ReadOnlyCollectionBuilder.Enumerator<object>
	// System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<object>
	// System.Runtime.CompilerServices.TaskAwaiter<byte>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.TrueReadOnlyCollection<object>
	// System.Span<ushort>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<byte>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<byte>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<byte>
	// System.Threading.Tasks.TaskFactory<object>
	// System.Tuple<int,float>
	// System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>
	// System.ValueTuple<byte,object>
	// System.ValueTuple<double,int>
	// System.ValueTuple<int,byte>
	// System.ValueTuple<int,int>
	// System.ValueTuple<object,object,object>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene,int>
	// UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>
	// }}

	public void RefMethods()
	{
		// Google.Protobuf.FieldCodec<object> Google.Protobuf.FieldCodec.ForMessage<object>(uint,Google.Protobuf.MessageParser<object>)
		// System.Collections.Generic.IList<byte> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<byte>(System.Collections.Generic.IList<byte>)
		// System.Collections.Generic.IList<double> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<double>(System.Collections.Generic.IList<double>)
		// System.Collections.Generic.IList<float> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<float>(System.Collections.Generic.IList<float>)
		// System.Collections.Generic.IList<int> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<int>(System.Collections.Generic.IList<int>)
		// System.Collections.Generic.IList<long> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<long>(System.Collections.Generic.IList<long>)
		// System.Collections.Generic.IList<object> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<object>(System.Collections.Generic.IList<object>)
		// System.Collections.Generic.IList<uint> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<uint>(System.Collections.Generic.IList<uint>)
		// System.Collections.Generic.IList<ulong> Google.ProtocolBuffers.Collections.Lists.AsReadOnly<ulong>(System.Collections.Generic.IList<ulong>)
		// bool Google.ProtocolBuffers.ICodedInputStream.ReadEnum<int>(int&,object&)
		// System.Void Google.ProtocolBuffers.ICodedInputStream.ReadEnumArray<int>(uint,string,System.Collections.Generic.ICollection<int>,System.Collections.Generic.ICollection<object>&)
		// System.Void Google.ProtocolBuffers.ICodedInputStream.ReadMessageArray<object>(uint,string,System.Collections.Generic.ICollection<object>,object,Google.ProtocolBuffers.ExtensionRegistry)
		// System.Void Google.ProtocolBuffers.ICodedOutputStream.WriteEnumArray<int>(int,string,System.Collections.Generic.IEnumerable<int>)
		// System.Void Google.ProtocolBuffers.ICodedOutputStream.WriteMessageArray<object>(int,string,System.Collections.Generic.IEnumerable<object>)
		// object LitJson.JsonMapper.ToObject<object>(string)
		// object System.Activator.CreateInstance<object>()
		// int System.Array.BinarySearch<object>(object[],int,int,object,System.Collections.Generic.IComparer<object>)
		// int System.Array.BinarySearch<object>(object[],object,System.Collections.Generic.IComparer<object>)
		// object[] System.Array.Empty<object>()
		// int System.Array.FindIndex<byte>(byte[],int,int,System.Predicate<byte>)
		// int System.Array.IndexOf<byte>(byte[],byte,int)
		// int System.Array.IndexOf<byte>(byte[],byte,int,int)
		// int System.Array.IndexOf<ushort>(ushort[],ushort)
		// int System.Array.IndexOfImpl<byte>(byte[],byte,int,int)
		// int System.Array.IndexOfImpl<ushort>(ushort[],ushort,int,int)
		// System.Void System.Array.Resize<BestHTTP.Core.ProgressFlattener.FlattenedProgress>(BestHTTP.Core.ProgressFlattener.FlattenedProgress[]&,int)
		// System.Void System.Array.Resize<byte>(byte[]&,int)
		// System.Void System.Array.Resize<int>(int[]&,int)
		// System.Void System.Array.Resize<object>(object[]&,int)
		// System.Void System.Array.Sort<object,object>(object[],object[],System.Collections.Generic.IComparer<object>)
		// System.Void System.Array.Sort<object,object>(object[],object[],int,int,System.Collections.Generic.IComparer<object>)
		// System.Void System.Array.Sort<object>(object[])
		// System.Void System.Array.Sort<object>(object[],System.Comparison<object>)
		// System.Void System.Array.Sort<object>(object[],int,int,System.Collections.Generic.IComparer<object>)
		// System.Collections.Generic.List<int> System.Collections.Generic.List<object>.ConvertAll<int>(System.Converter<object,int>)
		// System.Collections.Generic.List<object> System.Collections.Generic.List<int>.ConvertAll<object>(System.Converter<int,object>)
		// System.Collections.ObjectModel.ReadOnlyCollection<object> System.Dynamic.Utils.CollectionExtensions.ToReadOnly<object>(System.Collections.Generic.IEnumerable<object>)
		// bool System.Linq.Enumerable.All<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// bool System.Linq.Enumerable.Any<ushort>(System.Collections.Generic.IEnumerable<ushort>,System.Func<ushort,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
		// int System.Linq.Enumerable.Count<UnityEngine.Vector2>(System.Collections.Generic.IEnumerable<UnityEngine.Vector2>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Distinct<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Distinct<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.DistinctIterator<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.DistinctIterator<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEqualityComparer<object>)
		// UnityEngine.Vector2 System.Linq.Enumerable.ElementAt<UnityEngine.Vector2>(System.Collections.Generic.IEnumerable<UnityEngine.Vector2>,int)
		// int System.Linq.Enumerable.First<int>(System.Collections.Generic.IEnumerable<int>)
		// object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>)
		// BestHTTP.SocketIO3.Events.CallbackDescriptor System.Linq.Enumerable.FirstOrDefault<BestHTTP.SocketIO3.Events.CallbackDescriptor>(System.Collections.Generic.IEnumerable<BestHTTP.SocketIO3.Events.CallbackDescriptor>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<int,object>> System.Linq.Enumerable.GroupBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// int System.Linq.Enumerable.Max<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// int System.Linq.Enumerable.Min<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>> System.Linq.Enumerable.OrderBy<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>(System.Collections.Generic.IEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>,System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>)
		// System.Linq.IOrderedEnumerable<int> System.Linq.Enumerable.OrderBy<int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderByDescending<object,byte>(System.Collections.Generic.IEnumerable<object>,System.Func<object,byte>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderByDescending<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<object,object>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>(System.Collections.Generic.IEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>,System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// int System.Linq.Enumerable.Sum<int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
		// System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>> System.Linq.Enumerable.ThenBy<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>(System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>,System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>)
		// System.Linq.IOrderedEnumerable<int> System.Linq.Enumerable.ThenBy<int,int>(System.Linq.IOrderedEnumerable<int>,System.Func<int,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.ThenBy<object,int>(System.Linq.IOrderedEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.ThenBy<object,ulong>(System.Linq.IOrderedEnumerable<object>,System.Func<object,ulong>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.ThenByDescending<object,int>(System.Linq.IOrderedEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.KeyValuePair<object,object>[] System.Linq.Enumerable.ToArray<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,int>,int,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,int>,int,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<int,int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>,System.Func<int,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<int,int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>,System.Func<int,int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,object> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,object>,int,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>)
		// System.Collections.Generic.Dictionary<int,object> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,object>,int,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,object> System.Linq.Enumerable.ToDictionary<object,int,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,object>)
		// System.Collections.Generic.Dictionary<int,object> System.Linq.Enumerable.ToDictionary<object,int,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,object>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,object> System.Linq.Enumerable.ToDictionary<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.HashSet<int> System.Linq.Enumerable.ToHashSet<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.HashSet<int> System.Linq.Enumerable.ToHashSet<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.HashSet<ulong> System.Linq.Enumerable.ToHashSet<ulong>(System.Collections.Generic.IEnumerable<ulong>)
		// System.Collections.Generic.HashSet<ulong> System.Linq.Enumerable.ToHashSet<ulong>(System.Collections.Generic.IEnumerable<ulong>,System.Collections.Generic.IEqualityComparer<ulong>)
		// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>> System.Linq.Enumerable.ToList<System.Collections.Generic.KeyValuePair<int,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>)
		// System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<uint> System.Linq.Enumerable.ToList<uint>(System.Collections.Generic.IEnumerable<uint>)
		// System.Collections.Generic.List<ulong> System.Linq.Enumerable.ToList<ulong>(System.Collections.Generic.IEnumerable<ulong>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,object>>.Select<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>(System.Func<System.Collections.Generic.KeyValuePair<int,object>,System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<int>.Select<int>(System.Func<int,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>.Select<object>(System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>.Select<object>(System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,System.Linq.Expressions.ParameterExpression[])
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,bool,System.Collections.Generic.IEnumerable<System.Linq.Expressions.ParameterExpression>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,string,bool,System.Collections.Generic.IEnumerable<System.Linq.Expressions.ParameterExpression>)
		// System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>> System.Linq.IOrderedEnumerable<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>>.CreateOrderedEnumerable<int>(System.Func<System.ValueTuple<System.Collections.Generic.KeyValuePair<int,object>,int,int,int,int>,int>,System.Collections.Generic.IComparer<int>,bool)
		// System.Linq.IOrderedEnumerable<int> System.Linq.IOrderedEnumerable<int>.CreateOrderedEnumerable<int>(System.Func<int,int>,System.Collections.Generic.IComparer<int>,bool)
		// System.Linq.IOrderedEnumerable<object> System.Linq.IOrderedEnumerable<object>.CreateOrderedEnumerable<int>(System.Func<object,int>,System.Collections.Generic.IComparer<int>,bool)
		// System.Linq.IOrderedEnumerable<object> System.Linq.IOrderedEnumerable<object>.CreateOrderedEnumerable<ulong>(System.Func<object,ulong>,System.Collections.Generic.IComparer<ulong>,bool)
		// System.Linq.ParallelQuery<object> System.Linq.ParallelEnumerable.AsParallel<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Linq.ParallelQuery<int> System.Linq.ParallelEnumerable.Select<object,int>(System.Linq.ParallelQuery<object>,System.Func<object,int>)
		// System.Linq.ParallelQuery<ulong> System.Linq.ParallelEnumerable.Select<object,ulong>(System.Linq.ParallelQuery<object>,System.Func<object,ulong>)
		// object[] System.Linq.ParallelEnumerable.ToArray<object>(System.Linq.ParallelQuery<object>)
		// System.Collections.Generic.List<object> System.Linq.ParallelEnumerable.ToList<object>(System.Linq.ParallelQuery<object>)
		// System.Linq.ParallelQuery<object> System.Linq.ParallelEnumerable.Where<object>(System.Linq.ParallelQuery<object>,System.Func<object,bool>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.Assembly)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ChapterMapView.<SaveFogDataAsync1>d__179>(System.Runtime.CompilerServices.TaskAwaiter&,ChapterMapView.<SaveFogDataAsync1>d__179&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<ChapterMapView.<SaveFogDataAsync1>d__179>(ChapterMapView.<SaveFogDataAsync1>d__179&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,TapSDKManager.<InitializeTapSDK>d__24>(System.Runtime.CompilerServices.TaskAwaiter<object>&,TapSDKManager.<InitializeTapSDK>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,TapSDKManager.<TapSDKLogin>d__27>(System.Runtime.CompilerServices.TaskAwaiter<object>&,TapSDKManager.<TapSDKLogin>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<TapSDKManager.<InitializeTapSDK>d__24>(TapSDKManager.<InitializeTapSDK>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<TapSDKManager.<TapSDKLogin>d__27>(TapSDKManager.<TapSDKLogin>d__27&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Tuple<int,float> System.Tuple.Create<int,float>(int,float)
		// byte ThirdParty.Wrapper.ThirdPartyWrapper.CallJavaWithReturn<byte>(string,object[])
		// float ThirdParty.Wrapper.ThirdPartyWrapper.CallJavaWithReturn<float>(string,object[])
		// int ThirdParty.Wrapper.ThirdPartyWrapper.CallJavaWithReturn<int>(string,object[])
		// object ThirdParty.Wrapper.ThirdPartyWrapper.CallJavaWithReturn<object>(string,object[])
		// byte UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
		// float UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<float>(System.IntPtr)
		// int UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
		// object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<float>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// byte UnityEngine.AndroidJavaObject.Call<byte>(string,object[])
		// float UnityEngine.AndroidJavaObject.Call<float>(string,object[])
		// int UnityEngine.AndroidJavaObject.Call<int>(string,object[])
		// object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
		// byte UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<byte>(System.IntPtr)
		// float UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<float>(System.IntPtr)
		// int UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<int>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<object>(System.IntPtr)
		// byte UnityEngine.AndroidJavaObject._Call<byte>(string,object[])
		// float UnityEngine.AndroidJavaObject._Call<float>(string,object[])
		// int UnityEngine.AndroidJavaObject._Call<int>(string,object[])
		// object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
		// object UnityEngine.AssetBundle.LoadAsset<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object[] UnityEngine.Object.FindObjectsOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
		// object UnityEngine.Resources.Load<object>(string)
		// byte UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
		// float UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<float>(System.IntPtr)
		// int UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<float>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<byte>(object[])
		// string UnityEngine._AndroidJNIHelper.GetSignature<float>(object[])
		// string UnityEngine._AndroidJNIHelper.GetSignature<int>(object[])
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
	}
}