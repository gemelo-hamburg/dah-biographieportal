using Gemelo.Applications.Biographieportal.Code;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.EntrieDatas
{

    public class Fragment : IHasFileName
    {
        #region öffentliche Eigenschaften

        public string type { get; set; } = string.Empty;
        public LocalizationString text { get; set; }// = new();
        public string mediaType { get; set; }// = string.Empty;
        public LocalizationString copyright { get; set; }// = new();
        public string filename { get; set; }// = string.Empty;
        public Poster poster { get; set; }// = new();

        #endregion öffentliche Eigenschaften

        #region ctor

        public Fragment()
        {
            //part.PartType
        }

        public Fragment(BiographyLocalizationString str)
            : this()
        {
            text = new LocalizationString(str);
        }

        #endregion ctor

        #region öffentliche Methoden

        public static string GetWebMediaTypeString(MediaFileType mediaFileType)
        {
            return mediaFileType switch
            {
                MediaFileType.Video => "movie",
                MediaFileType.Image => "image",
                _ => "unknown"
            };
        }

        public static bool GetMediaExists(MediaFile mf)
        {
            if (mf == null) return false;
            else if (mf.FileInfo == null) return false;
            else if (mf.FileInfo.Name == BiographyReader.ConstMissingImagePlaceHolder) return false;
            else return true;
        }

        public static IEnumerable<Fragment> CreateFromPart(BiographyPart part)
        {
            List<Fragment> fragments = new List<Fragment>();

            CreateHeader(part, fragments);
            CreateText(part, fragments);
            CreateMedia(part, fragments);

            return fragments;
        }

        private static void CreateMedia(BiographyPart part, List<Fragment> fragments)
        {
            if (GetMediaExists(part.MediaFile))
            {
                Fragment fragment = new Fragment
                {
                    type = Entry.FragmentType_Media,
                    copyright = new LocalizationString(part.MediaFile.Copyright),
                    filename = part.MediaFile.FileInfo.Name,
                };

                switch (part.MediaFile.Type)
                {
                    case MediaFileType.Video:
                        fragment.mediaType = Entry.FragmentMediaType_Video;
                        fragment.poster = new Poster();
                        break;
                    case MediaFileType.Image:
                        fragment.mediaType = Entry.FragmentMediaType_Image;
                        break;
                }

                fragments.Add(fragment);
            }
        }

        private static void CreateText(BiographyPart part, List<Fragment> fragments)
        {
            if (!part.Text.IsEmpty)
            {
                fragments.Add(new Fragment(part.Text) { type = Entry.FragmentType_Text });
            }
        }

        private static void CreateHeader(BiographyPart part, List<Fragment> fragments)
        {
            if (!part.Title.IsEmpty)
            {
                fragments.Add(new Fragment(part.Title) { type = Entry.FragmentType_Headline });
            }
        }

        #endregion öffentliche Methoden
    }
}
