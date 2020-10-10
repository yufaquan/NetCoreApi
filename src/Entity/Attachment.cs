using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using SqlSugar;
using static Entity.Enums;

namespace Entity
{
    ///<summary>
    ///附件
    ///</summary>
    [YDisplay("附件")]
    [SugarTable("sys_attachment")]
    [KnownType(typeof(Attachment))]
    public partial class Attachment : BaseModel
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// base64
        /// </summary>
        public string Base64 { get; set; }
        /// <summary>
        /// 是否可用true：可用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string Extension { get; set; }


        public void GetFileType()
        {
            if (!string.IsNullOrWhiteSpace(ContentType))
            {
                var docs = new List<string> {"application/pdf",
                    "application/vnd.ms-excel",
                    "application/vnd.ms-powerpoint",
                    "text/css",
                    "text/html",
                    "text/plain",
                    "text/richtext",
                    "text/rtf",
                    "text/sgml",
                    "text/tab-separated-values",
                    "text/vnd.wap.wml",
                    "text/vnd.wap.wmlscript",
                    "text/x-setext",
                    "text/xml"};
                var images = new List<string> {"image/bmp",
                   "image/gif",
                   "image/ief",
                   "image/jpeg",
                   "image/jpeg",
                   "image/jpeg",
                   "image/png",
                   "image/tiff",
                   "image/tiff",
                   "image/vnd.djvu",
                   "image/vnd.djvu",
                   "image/vnd.wap.wbmp",
                   "image/x-cmu-raster",
                   "image/x-portable-anymap",
                   "image/x-portable-bitmap",
                   "image/x-portable-graymap",
                   "image/x-portable-pixmap",
                   "image/x-rgb",
                   "image/x-xbitmap",
                   "image/x-xpixmap",
                   "image/x-xwindowdump" };
                var audios = new List<string> { "audio/basic",
                   "audio/basic",
                   "audio/midi",
                   "audio/midi",
                   "audio/midi",
                   "audio/mpeg",
                   "audio/mpeg",
                   "audio/mpeg",
                   "audio/x-aiff",
                   "audio/x-aiff",
                   "audio/x-aiff",
                   "audio/x-mpegurl",
                   "audio/x-pn-realaudio",
                   "audio/x-pn-realaudio",
                   "audio/x-pn-realaudio-plugin",
                   "audio/x-realaudio",
                   "audio/x-wav"};
                var videos = new List<string> { "video/mpeg",
                   "video/mpeg",
                   "video/mpeg",
                   "video/quicktime",
                   "video/quicktime",
                   "video/vnd.mpegurl",
                   "video/x-msvideo",
                   "video/x-sgi-movie"};

                if (docs.Exists(x=>x== ContentType))
                {
                    Type = FileType.Document;
                }
                else if (images.Exists(x => x == ContentType))
                {
                    Type = FileType.Pictrue;
                }
                else if (audios.Exists(x => x == ContentType))
                {
                    Type = FileType.Voice;
                }
                else if (videos.Exists(x => x == ContentType))
                {
                    Type = FileType.Video;
                }
                else
                {
                    Type = FileType.Null;
                }

            }

           

           

           

           
        }
    }


}
