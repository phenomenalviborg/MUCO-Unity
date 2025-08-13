using System;
using System.Runtime.InteropServices;
namespace Muco
{
    public class NetworkDiscoverer {
        #if UNITY_EDITOR_LINUX
            private const string LIBRARY_NAME = "discover_server_linux";
        #else
            private const string LIBRARY_NAME = "discover_server";
        #endif
        [DllImport(LIBRARY_NAME)]
        unsafe private static extern char* hello();

        [DllImport(LIBRARY_NAME)]
        unsafe private static extern void* new_discoverer([MarshalAs(UnmanagedType.LPUTF8Str)] string serviceType);

        [DllImport(LIBRARY_NAME)]
        unsafe private static extern void destroy_discoverer(void* discoverer);

        [DllImport(LIBRARY_NAME)]
        unsafe private static extern char* try_discover(void* discoverer);

        unsafe void* my_discoverer;

        public string category = "Discover";

        public NetworkDiscoverer(string serviceType) {
            try {
                unsafe {
                    Muco.VrDebug.SetValue(category, "lib_name", LIBRARY_NAME);
                    Muco.VrDebug.SetValue(category, "Hello", "Started");
                    var c = hello();
                    Muco.VrDebug.SetValue(category, "Hello", "Ran");
                    string s = Marshal.PtrToStringAnsi((IntPtr)c);
                    Muco.VrDebug.SetValue(category, "Hello", s);

                    Muco.VrDebug.SetValue(category, "MakingDiscoverer", "Started");
                    my_discoverer = new_discoverer(serviceType);
                    Muco.VrDebug.SetValue(category, "MakingDiscoverer", "Finished");
                }
            }
            catch (Exception ex){
                Muco.VrDebug.SetValue(category, "Exception", ex.ToString());
            }
        }

        public string Poll() {
            string s = null;
            unsafe {
                if (my_discoverer != null) {
                    Muco.VrDebug.SetValue(category, "TryingToDiscover", "Started");
                    var c = try_discover(my_discoverer);
                    Muco.VrDebug.SetValue(category, "TryingToDiscover", "Ran");
                    if (c != null) {
                        Muco.VrDebug.SetValue(category, "TryingToDiscover", "FoundSomething");
                        s = Marshal.PtrToStringAnsi((IntPtr)c);
                        Muco.VrDebug.SetValue(category, "Server Found", s);
                        destroy_discoverer(my_discoverer);
                        my_discoverer = null;
                    }
                    Muco.VrDebug.SetValue(category, "TryingToDiscover", "Finished");
                }
            }
            return s;
        }

        public void Dispose() {
            unsafe {
                if (my_discoverer != null) {
                    destroy_discoverer(my_discoverer);
                    my_discoverer = null;
                }
            }
            Muco.VrDebug.SetValue(category, "MakingDiscoverer", "Destroyed");
        }
    }
}
