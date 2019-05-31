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

namespace POL_CORE
{
	class CleanCache
	{
		public static void clearApplicationData(Context ApplicationContext) 
		{
			Java.IO.File cache = ApplicationContext.CacheDir;
			Java.IO.File  appDir = new Java.IO.File(cache.Parent);
			if (appDir.Exists()) {
				String[] children = appDir.List();
				for (int i = 0; i < children.Length; i++) {
					if (!children[i].Equals("lib")) {
						deleteDir(new Java.IO.File(appDir, children[i]));

						//Log.i("TAG", "**************** File /data/data/APP_PACKAGE/" + children[i] + " DELETED *******************");
					}
				}
			}
		}

		public static bool deleteDir(Java.IO.File dir) 
		{
			if (dir != null && dir.IsDirectory) {
				String[] children = dir.List();
				for (int i = 0; i < children.Length; i++) {
					bool success = deleteDir(new Java.IO.File(dir, children[i]));
					if (!success) {
						return false;
					}
				}
			}
			return dir.Delete ();
		}

	}
}

