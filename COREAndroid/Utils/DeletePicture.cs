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
using Android.Provider;

namespace POL_CORE
{
	public class DeletePicture
	{

		ContentResolver _resolver;
		public DeletePicture(ContentResolver cte)
		{
			this._resolver = cte;

		}

	}
}

