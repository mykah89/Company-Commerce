using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.General
{
    public class JsonDataResultViewModel
    {
        private Boolean _success;

        public JsonDataResultViewModel()
        {
            Errors = new List<String>();
            _success = false;
        }

        public object Data { get; set; }

        public List<String> Errors { get; set; }

        public Boolean Success
        {
            get
            {
                return Errors.Count > 0 ? false : _success;
            }
            set
            {
                _success = value;
            }
        }
    }
}