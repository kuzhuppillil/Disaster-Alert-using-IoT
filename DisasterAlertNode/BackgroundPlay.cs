using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.Playback;
using Windows.Storage;

namespace DisasterAlertNode
{
    public sealed class BackgroundPlay : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;
            BackgroundMediaPlayer.MessageReceivedFromBackground += OnBackgroundMessageReceived;

        }

        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            BackgroundMediaPlayer.Shutdown();
            _deferral.Complete();   // Notify that the operation is completed.

        }

        private async void OnBackgroundMessageReceived(object sender, MediaPlayerDataReceivedEventArgs e)
        {

            try
            {
                String fileName = String.Format("{0}\\{1}", System.IO.Directory.GetCurrentDirectory(), "alert.wav");
                // BackgroundMediaPlayer.Current.AutoPlay = false;
                var istorage = await StorageFile.GetFileFromPathAsync(fileName);
                // BackgroundMediaPlayer.Current.MediaEnded += OnMediaPlayed;
                BackgroundMediaPlayer.Current.SetFileSource(istorage);
                BackgroundMediaPlayer.Current.Play();
                System.Diagnostics.Debug.WriteLine("File path to write details - {0}", fileName);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
