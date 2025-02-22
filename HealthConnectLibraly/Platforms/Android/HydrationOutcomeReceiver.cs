using Android.OS;
using Android.Runtime;

namespace HealthConnectLibraly.Platforms.Android
{
    public class HydrationOutcomeReceiver : Java.Lang.Object, IOutcomeReceiver
    {
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        private readonly Action<Java.Lang.Object?> _onResultCallback;

        public Task<bool> Task => _tcs.Task;

        public HydrationOutcomeReceiver( Action<Java.Lang.Object?> onResultCallback )
        {
            _onResultCallback = onResultCallback;
        }

        public void OnResult( Java.Lang.Object? result )
        {
            _onResultCallback?.Invoke( result );

            if( result != null )
            {
                _tcs.SetResult( true );
            }
            else
            {
                _tcs.SetResult( false );
            }
        }

        public new nint Handle => throw new NotImplementedException();
        public new void Dispose() { }
        public new void UnregisterFromRuntime() { }
        private static readonly JniHandleOwnership Transfer = JniHandleOwnership.TransferLocalRef;
        private static nint _class_ref;


        public static (nint jnienv, nint handle, JniHandleOwnership transfer) GetJniParameters()
        {
            var jnienv = JNIEnv.Handle;
            var handle = _class_ref;
            return (jnienv, handle, Transfer);
        }
    }
}
