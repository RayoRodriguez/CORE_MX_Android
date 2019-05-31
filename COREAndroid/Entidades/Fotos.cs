using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Runtime.Serialization;

namespace POL_CORE
{
	public class Fotos
	{

		private string _idODT;
		private string _url;
		private string _urlId;



		public string IdODT
		{
			set
			{
				_idODT = value;
			}
			get
			{
				return _idODT;
			}
		}

		public string Url
		{
			set
			{
				_url = value;
			}
			get
			{
				return _url;
			}
		}

		public string UrlId
		{
			set
			{
				_urlId = value;
			}
			get
			{
				return _urlId;
			}
		}

	}
}

