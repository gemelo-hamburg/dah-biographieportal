using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Pete.Components.Extensions.Extensions;
using Pete.Components.WpfExtensions.Extensions;
using Pete.Components.WpfExtensions.Helpers;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class MediaFile
    {
        #region Private Variablen

        private FileInfo m_FileInfo;

        #endregion Private Variablen

        #region Öffentliche Properties

        public FileInfo FileInfo => m_FileInfo;

        public MediaFileType Type { get; private set; }
        public ImageSource ImageSource { get; private set; }
        public BiographyLocalizationString Copyright { get; set; }
        public FileInfo VideoPreview { get; internal set; }

        #endregion Öffentliche Properties

        #region Events

        #endregion Events

        #region Konstruktur und Initialisierungen

        public MediaFile(FileInfo fi)
        {
            m_FileInfo = fi;
            InitializeThings();
        }

        public static MediaFile CreateEmpty()
        {
            return new MediaFile(null);
        }

        private void InitializeThings()
        {
            Copyright = new BiographyLocalizationString();

            if (m_FileInfo == null || !m_FileInfo.Exists) Type = MediaFileType.NotExist;
            else InitByFileExtension(m_FileInfo.Extension);

        }

        #endregion Konstruktur und Initialisierungen

        #region Öffentliche Methoden

        public void Normalize()
        {
            switch (Type)
            {
                case MediaFileType.Image:
                    ImageSource = ImageSourceHelper.CreateFromExternalPath(m_FileInfo.FullName);
                    ImageSource.Freeze();
                    break;
                case MediaFileType.Video:
                    if (VideoPreview != null && VideoPreview.Exists)
                    {
                        ImageSource = ImageSourceHelper.CreateFromExternalPath(VideoPreview.FullName);
                        ImageSource.Freeze();
                    }
                    break;
            }
        }

        #endregion Öffentliche Methoden

        #region Private Methoden

        public void InitByFileExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".png":
                    Type = MediaFileType.Image;
                    break;
                case ".avi":
                case ".mov":
                case ".mp4":
                    Type = MediaFileType.Video;
                    break;
                default:
                    Type = MediaFileType.NotExist;
                    break;
            }
        }

        #endregion Private Methoden

        #region Eventhandler

        #endregion Eventhandler


    }


}
