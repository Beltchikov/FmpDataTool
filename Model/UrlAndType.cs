using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool.Model
{
    /// <summary>
    /// UrlAndType
    /// </summary>
    public class UrlAndType
    {
        public string Url { get; set; }
        public string DocumentName { get; set; }
        public Type ReturnType { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Url))
                {
                    return string.Empty;
                }
                
                int idxQuestionMark = Url.IndexOf("?");
                int lastIndxOfSlash = Url.LastIndexOf("/");
                return Url[(lastIndxOfSlash+1)..idxQuestionMark];
            }
        }
    }
}
