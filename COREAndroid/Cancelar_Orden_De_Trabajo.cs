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
using Android.Graphics;
using System.IO;
using POL_CORE.core;
using System.ComponentModel;
using Android.Net;
using Java.Text;
using Android.Content.PM;
using System.Threading.Tasks;
using SAO.Entity.Operation;
using SAO.Bss.Helper;
using SAO.Entity.General;


namespace POL_CORE
{
		
	[Activity (Label = "",ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class Cancelar_Orden_De_Trabajo : Activity
	{
		private short ReasonId; 
		private byte[]bitmapDetalle=null;
		private TextView lblInfoFoto;
		DTOODTCustom ODTDet= new DTOODTCustom ();
		Button Medidas,Clausurado,Arrendamiento,Arte,Otros;
		OperationStatus Request = new OperationStatus ();
		private int idevidencia;
		private DateTime datetime;
		BackgroundWorker bk = new BackgroundWorker();
	

		private ProgressDialog _progressDialog;
		private static string imageurl;
		private static string imageurlId;
		private static bool memoria=false;
		int Contador=0;

		async Task<bool>  CargarFoto()
		{

			return await Task.Factory.StartNew (() =>
				{
					OperationClient WS= null;

					try{
						WS =Utils.InitializeServiceClient();
						Request= WS.UploadODTFotoForMovil(ODTDet.Folio,Utils.compress(Convert.ToBase64String(bitmapDetalle,Base64FormattingOptions.InsertLineBreaks)));


						if(Request.IsComplete &&Convert.ToInt32( Request.RecordId)!=0)
						{
							idevidencia=Convert.ToInt32(Request.RecordId);
							return true;

						}else
						{
							return false;
						}
					}catch(Exception ex)
					{
						return false;
					}
				});

		}
	
		protected override void OnResume ()
		{
			base.OnResume ();


			if (Contador <1) 
			{
				IniciarCamara ();

				Contador++;
			}

		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent evento)  
		{
			base.OnKeyDown (keyCode, evento);

			if (keyCode==Keycode.Back) 
			{
				Finish ();


			}
			return true;

		}



		protected override void OnSaveInstanceState (Bundle outState)
		{
			if(!string.IsNullOrEmpty( imageurl))
				outState.PutString ("pathImagen1", imageurl);

			if(!string.IsNullOrEmpty( imageurlId))
				outState.PutString ("pathImagen1Id", imageurlId);

			if(!string.IsNullOrEmpty( lblInfoFoto.Text))
				outState.PutString ("lblDetalle", lblInfoFoto.Text);

			outState.PutInt ("contador", Contador);


			base.OnSaveInstanceState (outState);
		}




		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);


			SetContentView (Resource.Layout.Cancelar_Orden_De_Trabajo);


			ImageButton ImgButton =  FindViewById<ImageButton> (Resource.Id.BtnRegresar);
			ImageButton ImgCamara =  FindViewById<ImageButton> (Resource.Id.BtnCamara);
			Medidas = FindViewById<Button> (Resource.Id.BtnMedidas);
			Clausurado = FindViewById<Button> (Resource.Id.BtnClausurado);
			Arrendamiento = FindViewById<Button> (Resource.Id.BtnArrendamiento);
			Arte = FindViewById<Button> (Resource.Id.BtnArte);
			Otros = FindViewById<Button> (Resource.Id.BtnOtros);
			Button Fotografia = FindViewById<Button> (Resource.Id.BtnFotografiaEvidencia);
			Button Cancelar = FindViewById<Button> (Resource.Id.BtnCancelarODT);


			//label
			lblInfoFoto = FindViewById<TextView> (Resource.Id.LblInfoFoto); 
			TextView lblODT = FindViewById<TextView> (Resource.Id.LblOrdenActual);



			ODTDet=(DTOODTCustom) Utils.Deserialize(typeof(DTOODTCustom),this.Intent.GetStringExtra ("CANCELAR_ODT"));
			lblODT.Text = ODTDet.Folio + " - " + ODTDet.NumeroVista;

			if (bundle != null) {


				if (bundle.ContainsKey("pathImagen1")) {
					imageurl = bundle.GetString("pathImagen1");
				}

				if (bundle.ContainsKey("pathImagen1Id")) {
					imageurlId = bundle.GetString("pathImagen1Id");
				}

				if (bundle.ContainsKey("lblDetalle")) {
					lblInfoFoto.Text = bundle.GetString("lblDetalle");
				}

				if (bundle.ContainsKey("contador")) {
					Contador = bundle.GetInt("contador");
				}

				if (!string.IsNullOrEmpty (imageurl)) {
					bitmapDetalle = ReloadFoto (imageurl);
					if (bitmapDetalle == null) 
					{
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía de evidencia correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
						return;
					}
				}


			}



			//Eventos

			Medidas.Click += delegate { PressMotive(5);};
			Clausurado.Click += delegate {PressMotive(6);};
			Arrendamiento.Click += delegate {PressMotive(7);};
			Arte.Click += delegate {PressMotive(8);};
			Otros.Click += delegate {PressMotive(9);};
			ImgButton.Click += delegate 
			{
				Finish();
			};

			ImgCamara.Click += delegate {
				IniciarCamara ();
			};
			Fotografia.Click += delegate {selectImage(2);};
			Cancelar.Click+=delegate{CancelarODT();};


			bk.DoWork += new DoWorkEventHandler(bk_DoWork);
			bk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bk_RunWorkerCompleted);
			_progressDialog = new ProgressDialog(this) { Indeterminate = true };
			_progressDialog.SetMessage("Por favor espere...");
			_progressDialog.SetCanceledOnTouchOutside (false);

			// Create your application here


			RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
		}




		public byte[] ReloadFoto( string imageUrl)
		{
			Android.Graphics.Bitmap bitmap;
			byte[] bitmapB = null;

			if (System.IO.File.Exists (imageUrl)) 
			{


				bitmap = Utils.ChangeSizeBitmap(BitmapFactory.DecodeFile (imageUrl),1600,1200);
				using (MemoryStream stream = new MemoryStream ()) {
					bitmap.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
					bitmapB = stream.ToArray ();
					stream.Flush ();
					stream.Close ();
				}

				bitmap.Dispose ();
			}

			return bitmapB;

		}


		string Error=string.Empty;
		void bk_DoWork(object sender, DoWorkEventArgs e)
		{



			if (Utils.Internet (ApplicationContext)) 
				{

				DTOODTCancelacion insert = new DTOODTCancelacion ();
				OperationClient WS = null;

					try
					{
						
						insert.ODTID = ODTDet.ID;
						insert.CuadrilleroID = Convert.ToInt32(iduser);
						insert.MotivoODTID = ReasonId;
						insert.Observaciones = comentarios;
						if (LoadFoto)
						{
							insert.FotoID = idevidencia;
							DTOFoto Evidencia= new DTOFoto();
							Evidencia.Nombre = lblInfoFoto.Text;
							Evidencia.FechaCreacion = datetime;
							insert.Foto = Evidencia;
							Request = null;
							WS=Utils.InitializeServiceClient();
							Request= WS.CancelODT(insert);

						}else
						{
							Request.IsComplete = true;
							memoria = true;
							DTOFoto Evidencia= new DTOFoto();
							Evidencia.Nombre = lblInfoFoto.Text;
							Evidencia.FechaCreacion = datetime;
							Evidencia.Foto=bitmapDetalle;
							insert.Foto = Evidencia;

							if (ODTDet.EsPospuesta) 
							{
								Utils.BorrarMemoriaPospuesta (ODTDet);
								ODTDet.EsPospuesta = false;

							}
							List<DTOODTCancelacion> MemCan = Utils.MemoriaCancelacion;

							if (MemCan != null)
							{
								MemCan.Add (insert);
								Utils.MemoriaCancelacion = MemCan;

							} else
							{
								List<DTOODTCancelacion> Mem = new List<DTOODTCancelacion>(); 
								Mem.Add (insert);
								Utils.MemoriaCancelacion = Mem;
							}

							List<Fotos> MemFotos = null;
							try{
								MemFotos = Utils.FotosMemoria;
							}
							catch(Exception exc)
							{
								throw exc;
							}

							if (MemFotos != null) 
							{
								Fotos f = new Fotos ();
								f.IdODT=ODTDet.ID.ToString();
								f.Url = imageurl;
								MemFotos.Add (f);

								Utils.FotosMemoria = MemFotos;

							} else
							{
								List<Fotos> Mem = new List<Fotos>(); 
								Fotos f = new Fotos ();
								f.IdODT=ODTDet.ID.ToString();
								f.Url = imageurl;
								Mem.Add (f);
								Utils.FotosMemoria = Mem;
							}

						}

					}
					catch(Exception exc) 
					{
					
						Error = "ODT:" + insert.ODTID.ToString () + "Error:" + exc.Message;


					}
					finally {
					if (WS != null)
						WS.Close ();
					}



				}
				else
				{
					memoria = true;
					DTOODTCancelacion insert = new DTOODTCancelacion ();
					Request.IsComplete = true;
					insert.ODTID = ODTDet.ID;
					insert.CuadrilleroID = Convert.ToInt32(iduser);
					insert.MotivoODTID = ReasonId;
					insert.Observaciones = comentarios;
					DTOFoto Evidencia= new DTOFoto();
					Evidencia.Foto = bitmapDetalle;
					Evidencia.FechaCreacion = datetime;
					insert.Foto = Evidencia;
					if (ODTDet.EsPospuesta) 
					{
						Utils.BorrarMemoriaPospuesta (ODTDet);
						ODTDet.EsPospuesta = false;

					}
				List<DTOODTCancelacion> MemCan = Utils.MemoriaCancelacion;

				if (MemCan != null)
				 {
					MemCan.Add (insert);
					Utils.MemoriaCancelacion = MemCan;

				} else
					{
						List<DTOODTCancelacion> Mem = new List<DTOODTCancelacion>(); 
						Mem.Add (insert);
						Utils.MemoriaCancelacion = Mem;
					}

				List<Fotos> MemFotos = null;

				try{
					MemFotos = Utils.FotosMemoria;
				}
				catch(Exception exc)
				{
					throw exc;
				}

				if (MemFotos != null) 
				{
					Fotos f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageurl;
					MemFotos.Add (f);
				
					Utils.FotosMemoria = MemFotos;

				} else
				{
					List<Fotos> Mem = new List<Fotos>(); 
					Fotos f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageurl;
					Mem.Add (f);
					Utils.FotosMemoria = Mem;
				}


				}


		}
		void bk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
					Toast toast;		
				    if (Request.IsComplete) 
					{
				
						Utils.ActualizarMemoria (ODTDet,117);
						bitmapDetalle = null;
						SetResult(Result.Ok);
						Finish();
				        _progressDialog.Hide();
						toast = Toast.MakeText(ApplicationContext, "La orden de trabajo se canceló correctamente",ToastLength.Long);
						toast.Show ();

					}else
					{
						if (!string.IsNullOrEmpty (Request.Message)) 
						{
							_progressDialog.Hide();
							toast = Toast.MakeText(ApplicationContext, Request.Message,ToastLength.Long);
							toast.Show ();
							return;
						}

						if (!string.IsNullOrEmpty (Error)) 
						{
							_progressDialog.Hide();
							toast = Toast.MakeText(ApplicationContext, Error,ToastLength.Long);
							toast.Show ();
							return;
						}

						
					}
		}
	
		private void PressMotive(short motive)
		{
			ReasonId = motive;

			switch(motive)
			{
			case 5:

				Medidas.SetBackgroundResource (Resource.Drawable.ButtonPresses);
				Clausurado.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arrendamiento.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arte.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Otros.SetBackgroundResource (Resource.Drawable.Blue_Button);
				break;
			case 6:
				Medidas.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Clausurado.SetBackgroundResource (Resource.Drawable.ButtonPresses);
				Arrendamiento.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arte.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Otros.SetBackgroundResource (Resource.Drawable.Blue_Button);
				break;
			case 7:
				Medidas.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Clausurado.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arrendamiento.SetBackgroundResource (Resource.Drawable.ButtonPresses);
				Arte.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Otros.SetBackgroundResource (Resource.Drawable.Blue_Button);
				break;
			case 8:
				Medidas.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Clausurado.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arrendamiento.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arte.SetBackgroundResource (Resource.Drawable.ButtonPresses);
				Otros.SetBackgroundResource (Resource.Drawable.Blue_Button);
				break;
			case 9:
				Medidas.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Clausurado.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arrendamiento.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Arte.SetBackgroundResource (Resource.Drawable.Blue_Button);
				Otros.SetBackgroundResource (Resource.Drawable.ButtonPresses);
				break;

			}


		}



		private void CancelarODT()
		{
			AlertDialog.Builder builder;
			builder = new AlertDialog.Builder(this);
			builder.SetTitle("Orden de trabajo");
			builder.SetMessage("¿Desea cancelar esta ODT?");
			builder.SetNegativeButton("No", delegate { builder.SetCancelable(true);});
			builder.SetPositiveButton("Si", delegate { CancelarODTWS(); });
			builder.Show();

		}


		private string comentarios;
		private string iduser;
		private async void CancelarODTWS()
		{
			comentarios= FindViewById<TextView> (Resource.Id.TxtComentarios).Text;
			Toast toast;
			iduser = Utils.UsuarioApp.Iduser;
			if (ReasonId == 0) 
			{
				toast = Toast.MakeText (ApplicationContext, "Debes de seleccionar un motivo para cancelar la ODT", ToastLength.Long);
				toast.Show ();
				return;
			} else if (String.IsNullOrEmpty (comentarios))
			{
				toast = Toast.MakeText (ApplicationContext, "Debes de introducir los comentarios para cancelar la ODT", ToastLength.Long);
				toast.Show ();
				return;

			}
			else if (bitmapDetalle==null)
			{
				toast = Toast.MakeText (ApplicationContext, "Debes de seleccionar una fotografía de evidencia para cancelar la ODT", ToastLength.Long);
				toast.Show ();
				return;
			}

			_progressDialog.Show();
			if (Utils.Internet (ApplicationContext)) 
				LoadFoto=await CargarFoto ();
			bk.RunWorkerAsync ();

		}

		bool LoadFoto=false;



		private void IniciarCamara( )
		{  


			if (IsThereAnAppToTakePictures()) {


				CreateDirectoryForPictures ();

				Intent intent = new Intent (MediaStore.ActionImageCapture);
				App._file = new Java.IO.File (App._dir, ODTDet.Folio + DateTime.Now.Day.ToString () + DateTime.Now.Month.ToString () + DateTime.Now.Year.ToString () + "-" + DateTime.Now.Hour.ToString () + DateTime.Now.Minute.ToString () + DateTime.Now.Second + "_1.jpg");
				intent.PutExtra (MediaStore.ExtraOutput, Android.Net.Uri.FromFile (App._file));
				StartActivityForResult (intent, 4);


			}



		} 



		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}


		private void CreateDirectoryForPictures()
		{


			App._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim), "Camera");


			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
		}

		private void selectImage(int opcion) 
		{

			Intent intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
			StartActivityForResult(intent, opcion);
		}
		
		public String getRealPathFromURI(Android.Net.Uri contentUri) {
			String[] proj = { MediaStore.MediaColumns.Data };
			var cursor = ManagedQuery(contentUri, proj, null, null, null);
			int column_index = cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.Data);
			cursor.MoveToFirst();
			string regreso=cursor.GetString(column_index);

			return regreso;
		}




		protected override void OnDestroy ()
		{
			base.OnDestroy();
			CleanCache.clearApplicationData (ApplicationContext);

		}


		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok) 
			{
				if (requestCode == 2)
				 {
					try
					{

							Android.Net.Uri selectImage = data.Data;
							bitmapDetalle = null;
							imageurl = getRealPathFromURI (data.Data);
							imageurlId = data.Data.LastPathSegment;
							datetime = System.IO.File.GetLastAccessTimeUtc (imageurl);
							string fechahora = datetime.Day.ToString () + datetime.Month.ToString () + datetime.Year.ToString ().Substring (2, 2) + "-" + datetime.Hour.ToString () + datetime.Minute.ToString ();
							
							GC.Collect ();
							if (File.Exists (imageurl))
						 {
							Android.Graphics.Bitmap bitmap = Utils.ChangeSizeBitmap(BitmapFactory.DecodeFile (imageurl),1600,1200);

								using (MemoryStream stream = new MemoryStream ()) 
								{
								bitmap.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
									bitmapDetalle = stream.ToArray ();
									stream.Flush ();
									stream.Close ();
								}
								
								bitmap.Dispose ();
							if (bitmapDetalle == null) 
							{
								Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
								return;
							}else
							{
								lblInfoFoto.Text = ODTDet.Folio + " - Evidencia - " + fechahora + ".jpg";
							}
							}

					}
					catch(Exception exc)
					{
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
						return;

					}

				
				} else if(requestCode == 4)
				{
					if (App._file.Exists ()) 
					{
						Intent mediaScannerIntent = new Intent (Intent.ActionMediaScannerScanFile);
						Android.Net.Uri contentUri = Android.Net.Uri.FromFile (App._file);
						mediaScannerIntent.SetData (contentUri);
						this.SendBroadcast (mediaScannerIntent);


						GC.Collect ();
					}
				}


			}
		} 

	}
}

