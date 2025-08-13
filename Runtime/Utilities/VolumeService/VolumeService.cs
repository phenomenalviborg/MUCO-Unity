namespace Muco {
    public class VolumeService {
        static VolumeService _volumeService;
        static VolumeService TheVolumeService => GetCachedVolumeService();

        public static float SystemVolume {
            get {
                return TheVolumeService.GetSystemVolume();
            }
            set {
                TheVolumeService.SetSystemVolume(value);
            }
        }

        static VolumeService GetCachedVolumeService() {
            if (_volumeService is VolumeService volumeService)
                return volumeService;
            var platformVolumeService = CreatePlatformVolumeServece();
            _volumeService = platformVolumeService;
            return platformVolumeService;
        }

        static VolumeService CreatePlatformVolumeServece() {
            if (PlatformDetection.ThePlatform.IsAndroid())
                return new AndroidNativeVolumeService();
            else
                return new VolumeService();
        }

        public virtual void SetSystemVolume(float volumeValue) {}
        public virtual float GetSystemVolume() { return 0f; }
    }
}
