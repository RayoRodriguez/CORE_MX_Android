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
using Java.IO;
using POL_CORE.core;
using System.ComponentModel;
using Android.Net;
using System.Threading;
using Android.Content.PM;
using System.IO;
using Android.Util;
using System.Threading.Tasks;
using SAO.Entity.Operation;
using System.Web.Services.Description;
using SAO.Bss.Helper;
using SAO.Entity.General;


namespace POL_CORE
{
	public static class App{
		public static Java.IO.File _file;
		public static Java.IO.File _dir;     
		public static Bitmap bitmap;
	}

	[Activity (Label = "Finalizar_Orden_De_Trabajo",ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class Finalizar_Orden_De_Trabajo : Activity
	{

		DTOODTCustom ODTDet= new DTOODTCustom ();


		private TextView lblFotoDetalle;
		private TextView lblFotoMediana;
		private TextView lblFotoLarga;
		private byte[] bitmapDetalle = null;
		private byte[] bitmapMediana = null;
		private byte[] bitmapLarga = null;
		private ProgressDialog _progressDialog;
		private DateTime datetimedetalle,datetimemediana,datetimelarga;
		OperationStatus Request = new OperationStatus ();
		private int iddetalle=0, idmediana=0, idlarga=0;
		private string imageUrl1;
		private string imageUrl2;
		private string imageUrl3;
		private static string imageurlId1;
		private static string imageurlId2;
		private static string imageurlId3;
		private bool memoria=false;

		BackgroundWorker bk = new BackgroundWorker();
		BackgroundWorker bk2 = new BackgroundWorker();
		public override bool OnKeyDown(Keycode keyCode, KeyEvent evento)  
		{
			base.OnKeyDown (keyCode, evento);

			if (keyCode==Keycode.Back) 
			{
				Finish ();


			}
			return true;

		}


//		async Task<bool> CargarFoto( string folio,byte[] bitmapDetalle, byte[] bitmapMediana,byte[] bitmapLarga)
//		{
//			bool Load1 = false, Load2 = false, Load3 = false;
//
//			Load1=await Task.Factory.StartNew (() =>
//				{
//					OperationClient WS=null;
//
//					try{
//						WS=Utils.InitializeServiceClient();
//						var r= WS.UploadODTFotoForMovil(folio,Utils.compress(Convert.ToBase64String(bitmapDetalle,Base64FormattingOptions.InsertLineBreaks)));
//
//
//						if (r.IsComplete && Convert.ToInt32 (r.RecordId) != 0) 
//						{
//							iddetalle=Convert.ToInt32(r.RecordId);
//
//							return true;		
//						}else
//						{
//							return false;
//						}
//					}catch(Exception ex)
//					{
//						return false;
//					}finally
//					{
//						if(WS!=null)
//							WS.Close();
//					}
//				});
//
//			Load2= await Task.Factory.StartNew (() =>
//				{
//					OperationClient WS=null;
//					try{
//
//						WS=Utils.InitializeServiceClient();
//						var r2= WS.UploadODTFotoForMovil(folio,Utils.compress(Convert.ToBase64String(bitmapMediana,Base64FormattingOptions.InsertLineBreaks)));
//						//var r2=WS.UploadODTFoto(folio,bitmapMediana);
//						if ( r2.IsComplete && Convert.ToInt32 (r2.RecordId) != 0) 
//						{
//							idmediana=Convert.ToInt32(r2.RecordId);
//							return true;		
//						}else
//						{
//							return false;
//						}
//					}catch(Exception ex)
//					{
//						return false;
//					}
//					finally
//					{
//						if(WS!=null)
//							WS.Close();
//					}
//				});
//
//
//			Load3= await Task.Factory.StartNew (() =>
//				{
//					OperationClient WS=null;
//					try{
//						WS=Utils.InitializeServiceClient();
//						var r3= WS.UploadODTFotoForMovil(folio,Utils.compress(Convert.ToBase64String(bitmapLarga,Base64FormattingOptions.InsertLineBreaks)));
//						//var r3= WS.UploadODTFoto(folio,bitmapLarga);
//
//						if (r3.IsComplete && Convert.ToInt32 (r3.RecordId) != 0) 
//						{
//							idlarga=Convert.ToInt32(r3.RecordId);
//							return true;		
//						}else
//						{
//							return false;
//						}
//					}catch(Exception ex)
//					{
//						return false;
//					}
//					finally
//					{
//						if(WS!=null)
//							WS.Close();
//					}
//				});
//
//			if (Load1 && Load2 && Load3)
//				return true;
//			else
//				return false;
//		}


		int Contador=0;

		protected override void OnResume ()
		{
			base.OnResume ();

			if (Contador <3) 
			{
				IniciarCamara ();
				Contador++;
			}

		}


		protected override void OnDestroy ()
		{
			base.OnDestroy();
			CleanCache.clearApplicationData (ApplicationContext);

		}
			
		protected override void OnSaveInstanceState (Bundle outState)
		{
			try{
				if(!string.IsNullOrEmpty( imageUrl1))
					outState.PutString ("pathImagen1", imageUrl1);

				if(!string.IsNullOrEmpty( imageurlId1))
					outState.PutString ("pathImagen1Id", imageurlId1);

				if(!string.IsNullOrEmpty( lblFotoDetalle.Text))
					outState.PutString ("lblDetalle", lblFotoDetalle.Text);

				if(!string.IsNullOrEmpty( imageUrl2))
					outState.PutString ("pathImagen2", imageUrl2);

				if(!string.IsNullOrEmpty( imageurlId2))
					outState.PutString ("pathImagen2Id", imageurlId2);

				if(!string.IsNullOrEmpty( lblFotoMediana.Text))
					outState.PutString ("lblMediana", lblFotoMediana.Text);

				if(!string.IsNullOrEmpty( imageUrl3))
					outState.PutString ("pathImagen3", imageUrl3);

				if(!string.IsNullOrEmpty( imageurlId3))
					outState.PutString ("pathImagen3Id", imageurlId3);


				if(!string.IsNullOrEmpty( lblFotoLarga.Text))
					outState.PutString ("lblLarga", lblFotoLarga.Text);

				outState.PutInt ("contador", Contador);
			}catch(Exception ex) {
				var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
				toast.Show ();
			}
			base.OnSaveInstanceState (outState);
		}


	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			try
			{
			RequestWindowFeature (WindowFeatures.NoTitle);

			SetContentView (Resource.Layout.Finalizar_Orden_De_Trabajo);

			//Botones
			ImageButton ImgButton =  FindViewById<ImageButton> (Resource.Id.BtnRegresar);
			ImageButton ImgCamara =  FindViewById<ImageButton> (Resource.Id.BtnCamara);
			Button btnFotoDetalle = FindViewById<Button> (Resource.Id.BtnFotografiaDetalle);
			Button btnFotoMediana = FindViewById<Button> (Resource.Id.BtnFotografiaMediana);
			Button btnFotoLarga = FindViewById<Button> (Resource.Id.BtnFotografiaLarga);
			Button btnFinalizar = FindViewById<Button> (Resource.Id.BtnFinalizar);

			//Labels
			lblFotoDetalle = FindViewById<TextView> (Resource.Id.LblInfoFotoDetalle);
			lblFotoMediana=FindViewById<TextView> (Resource.Id.LblInfoFotoMediana);;
			lblFotoLarga=FindViewById<TextView> (Resource.Id.LblInfoFotoLarga);;

			TextView lblODT = FindViewById<TextView> (Resource.Id.lblOrdenActual);

			ODTDet=(DTOODTCustom) Utils.Deserialize(typeof(DTOODTCustom),this.Intent.GetStringExtra ("ODT"));
			lblODT.Text = ODTDet.Folio + " - " + ODTDet.NumeroVista;


			if (bundle != null) {

				if (bundle.ContainsKey("pathImagen1")) {
					imageUrl1 = bundle.GetString("pathImagen1");
				}

				if (bundle.ContainsKey("pathImagen1Id")) {
					imageurlId1 = bundle.GetString("pathImagen1Id");
				}

				if (bundle.ContainsKey("lblDetalle")) {
					lblFotoDetalle.Text = bundle.GetString("lblDetalle");
				}

				if (!string.IsNullOrEmpty (imageUrl1)) {
					bitmapDetalle = ReloadFoto (imageUrl1);
					if (bitmapDetalle == null) 
					{
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía detalle correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
						return;
					}
				}

				if (bundle.ContainsKey("pathImagen2")) {
					imageUrl2 = bundle.GetString("pathImagen2");
				}

				if (bundle.ContainsKey("pathImagen2Id")) {
					imageurlId2 = bundle.GetString("pathImagen2Id");
				}

				if (bundle.ContainsKey("lblMediana")) {
					lblFotoMediana.Text = bundle.GetString("lblMediana");
				}

				if (!string.IsNullOrEmpty (imageUrl2)) {
					bitmapMediana = ReloadFoto (imageUrl2);

					if (bitmapMediana == null) 
					{
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía mediana correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
						return;
					}
				}

				if (bundle.ContainsKey("pathImagen3")) {
					imageUrl3 = bundle.GetString("pathImagen3");
				}

				if (bundle.ContainsKey("pathImagen3Id")) {
					imageurlId3 = bundle.GetString("pathImagen3Id");
				}

				if (bundle.ContainsKey("lblLarga")) {
					lblFotoLarga.Text = bundle.GetString("lblLarga");
				}

				if (!string.IsNullOrEmpty (imageUrl3)) {
					bitmapLarga = ReloadFoto (imageUrl3);

					if (bitmapLarga == null) 
					{
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía larga correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show();
						return;
					}
				}


				if (bundle.ContainsKey("contador")) {
					Contador = bundle.GetInt("contador");
				}



			}
			


			//Eventos
			ImgButton.Click += delegate 
							{ 	
										Finish();
							};
            btnFotoDetalle.Click += delegate { selectImage( 1); };
			btnFotoMediana.Click += delegate { selectImage( 2); };
			btnFotoLarga.Click += delegate { selectImage( 3); };
			btnFinalizar.Click+= delegate { FinalizarODT(); };
			ImgCamara.Click += delegate {
				IniciarCamaraButton ();
			};
		
		
			bk.DoWork += new DoWorkEventHandler(bk_DoWork);
			bk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bk_RunWorkerCompleted);
			_progressDialog = new ProgressDialog(this) { Indeterminate = true };
			_progressDialog.SetMessage("Por favor espere...");
			_progressDialog.SetCanceledOnTouchOutside (false);
			RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
			}catch(Exception ex) {
				var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
				toast.Show ();
			}
		}



		string errr=string.Empty;
		void bk_DoWork(object sender, DoWorkEventArgs e)
		{

			if (Utils.Internet (ApplicationContext)) 
			{
				OperationClient WS = null;
				try
				{
					DTOODTEjecucion insert = new DTOODTEjecucion ();
					insert.ODTID = ODTDet.ID;
					insert.CuadrilleroID = Convert.ToInt32(iduser);
					insert.Observaciones = comentarios;

					if(LoadFotos)
					{
						insert.CortaFotoID = iddetalle;
						insert.MediaFotoID = idmediana;
						insert.LargaFotoID = idlarga;
						DTOFoto Detalle = new DTOFoto ();
						Detalle.FechaCreacion = datetimedetalle;
						Detalle.Nombre = lblFotoDetalle.Text;
						insert.CortaFoto = Detalle;
						DTOFoto Mediana = new DTOFoto ();
						Mediana.FechaCreacion = datetimemediana;
						Mediana.Nombre = lblFotoMediana.Text;
						insert.MediaFoto = Mediana;
						DTOFoto Larga = new DTOFoto ();
						Larga.FechaCreacion = datetimelarga;
						Larga.Nombre = lblFotoLarga.Text;
						insert.LargaFoto = Larga;
						Request = null;
						WS=Utils.InitializeServiceClient();
						Request= WS.InsertODTEjecucion(insert);
					}else
					{


						Request.IsComplete = true;
						memoria = true;
						DTOFoto Detalle = new DTOFoto ();
						Detalle.Foto = bitmapDetalle;
						Detalle.FechaCreacion = datetimedetalle;
						DTOFoto Mediana = new DTOFoto ();
						Mediana.Foto = bitmapMediana;
						Mediana.FechaCreacion = datetimemediana;
						DTOFoto Larga = new DTOFoto ();
						Larga.Foto = bitmapLarga;
						Larga.FechaCreacion = datetimelarga;
						insert.CortaFoto = Detalle;
						insert.MediaFoto = Mediana;
						insert.LargaFoto = Larga;
						if (ODTDet.EsPospuesta) 
						{
							Utils.BorrarMemoriaPospuesta (ODTDet);
							ODTDet.EsPospuesta = false;
						}


						List<DTOODTEjecucion> MemEje = null;
						try{
							MemEje = Utils.MemoriaEjecucion;
						}
						catch(Exception exc)
						{
							errr= exc.Message;

						}

						List<Fotos> MemFotos = null;

						try{
							MemFotos = Utils.FotosMemoria;
						}
						catch(Exception exc)
						{
							errr= exc.Message;

						}

						if (MemEje != null) 
						{
							MemEje.Add (insert);
							Utils.MemoriaEjecucion = MemEje;

						} else
						{
							List<DTOODTEjecucion> Mem = new List<DTOODTEjecucion>(); 
							Mem.Add (insert);
							Utils.MemoriaEjecucion = Mem;
						}

						if (MemFotos != null) 
						{
							Fotos f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl1;
							f.UrlId = imageurlId1;
							MemFotos.Add (f);
							f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl2;
							f.UrlId = imageurlId2;
							MemFotos.Add (f);
							f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl3;
							f.UrlId = imageurlId3;
							MemFotos.Add (f);
							Utils.FotosMemoria = MemFotos;

						} else
						{
							List<Fotos> MemF = new List<Fotos>(); 
							Fotos f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl1;
							f.UrlId = imageurlId1;
							MemF.Add (f);
							f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl2;
							f.UrlId = imageurlId2;
							MemF.Add (f);
							f = new Fotos ();
							f.IdODT=ODTDet.ID.ToString();
							f.Url = imageUrl3;
							f.UrlId = imageurlId3;
							MemF.Add (f);
							Utils.FotosMemoria = MemF;
						}

					}

				}
				catch(Exception ex) {

					errr = ex.Message;
				}
				finally {

					if (WS != null)
						WS.Close ();
				}

			} else 
			{
				memoria = true;
				DTOODTEjecucion insert = new DTOODTEjecucion ();
				Request.IsComplete = true;
				insert.ODTID = ODTDet.ID;
				insert.CuadrilleroID = Convert.ToInt32(iduser);
				insert.Observaciones = comentarios;

				DTOFoto Detalle = new DTOFoto ();
				Detalle.Foto = bitmapDetalle;
				Detalle.FechaCreacion = datetimedetalle;
				DTOFoto Mediana = new DTOFoto ();
				Mediana.Foto = bitmapMediana;
				Mediana.FechaCreacion = datetimemediana;

				DTOFoto Larga = new DTOFoto ();
				Larga.Foto = bitmapLarga;
				Larga.FechaCreacion = datetimelarga;
				insert.CortaFoto = Detalle;
				insert.MediaFoto = Mediana;
				insert.LargaFoto = Larga;
				if (ODTDet.EsPospuesta) 
				{
					Utils.BorrarMemoriaPospuesta (ODTDet);
					ODTDet.EsPospuesta = false;
				}


				List<DTOODTEjecucion> MemEje = null;
				try{
					MemEje = Utils.MemoriaEjecucion;
				}
				catch(Exception exc)
				{
					errr= exc.Message;

				}

				List<Fotos> MemFotos = null;

				try{
					MemFotos = Utils.FotosMemoria;
				}
				catch(Exception exc)
				{
					errr= exc.Message;

				}

				if (MemEje != null) 
				{
					MemEje.Add (insert);
					Utils.MemoriaEjecucion = MemEje;

				} else
				{
					List<DTOODTEjecucion> Mem = new List<DTOODTEjecucion>(); 
					Mem.Add (insert);
					Utils.MemoriaEjecucion = Mem;
				}

				if (MemFotos != null) 
				{
					Fotos f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl1;
					f.UrlId = imageurlId1;
					MemFotos.Add (f);
					f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl2;
					f.UrlId = imageurlId2;
					MemFotos.Add (f);
					f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl3;
					f.UrlId = imageurlId3;
					MemFotos.Add (f);
					Utils.FotosMemoria = MemFotos;

				} else
				{
					List<Fotos> MemF = new List<Fotos>(); 
					Fotos f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl1;
					f.UrlId = imageurlId1;
					MemF.Add (f);
					f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl2;
					f.UrlId = imageurlId2;
					MemF.Add (f);
					f = new Fotos ();
					f.IdODT=ODTDet.ID.ToString();
					f.Url = imageUrl3;
					f.UrlId = imageurlId3;
					MemF.Add (f);
					Utils.FotosMemoria = MemF;
				}


			}


		}


		void bk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Toast toast;

			if(Request.IsComplete)
			{

				Utils.ActualizarMemoria (ODTDet,114);
				bitmapLarga = null;
				bitmapMediana = null;
				bitmapDetalle = null;
				SetResult (Result.Ok);
				Finish ();
				_progressDialog.Hide();
				toast = Toast.MakeText (ApplicationContext, "La orden de trabajo se finalizó  correctamente", ToastLength.Long);
				toast.Show ();

			}else
			{
				_progressDialog.Hide();
				if (!string.IsNullOrEmpty (Request.Message)) {
					toast = Toast.MakeText (ApplicationContext, Request.Message, ToastLength.Long);
					toast.Show ();
				}

				if (!string.IsNullOrEmpty (errr)) {
					toast = Toast.MakeText (ApplicationContext, errr, ToastLength.Long);
					toast.Show ();
				}

			}


		}


		private void IniciarCamaraButton()
		{
			try
			{
			CleanCache.clearApplicationData (ApplicationContext);
			IniciarCamara ();
			}catch(Exception ex) {
				var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
				toast.Show ();
			}

		}


		private void FinalizarODT()
		{
			AlertDialog.Builder builder;
			builder = new AlertDialog.Builder(this);
			builder.SetTitle("Orden de trabajo");
			builder.SetMessage("¿Desea finalizar esta ODT?");
			builder.SetNegativeButton("No", delegate { builder.SetCancelable(true);});
			builder.SetPositiveButton("Si", delegate { FinalizaODTWS(); });
			builder.Show();

		}

		private string comentarios;
		private string iduser;
		private async void FinalizaODTWS()
		{
			try{
				comentarios= FindViewById<TextView> (Resource.Id.TxtComentarios).Text;
				Toast toast;
			    iduser = Utils.UsuarioApp.Iduser;

				if (bitmapDetalle == null) {
					toast = Toast.MakeText (ApplicationContext, "Debes de seleccionar la fotografía detalle para finalizar la ODT", ToastLength.Long);
					toast.Show ();
					return;

				} else if (bitmapMediana == null) {
					toast = Toast.MakeText (ApplicationContext, "Debes de seleccionar la fotografía mediana para finalizar la ODT", ToastLength.Long);
					toast.Show ();
					return;
				} else if (bitmapLarga == null) {
					toast = Toast.MakeText (ApplicationContext, "Debes de seleccionar la fotografía larga para finalizar la ODT", ToastLength.Long);
					toast.Show ();
					return;

				} 


				_progressDialog.Show();

				if (Utils.Internet (ApplicationContext))
				{

					iddetalle = await Utils.CargarFoto (ODTDet.ID.ToString(), bitmapDetalle,ApplicationContext,DateTime.Now.ToString());
					idmediana = await Utils.CargarFoto (ODTDet.ID.ToString(), bitmapMediana,ApplicationContext,DateTime.Now.ToString());
					idlarga = await Utils.CargarFoto (ODTDet.ID.ToString(),bitmapLarga,ApplicationContext,DateTime.Now.ToString());
					if(iddetalle!=0 && idmediana!=0 && idlarga!=0)
					{
						LoadFotos=true;
					}

				}
				bk.RunWorkerAsync ();

			}catch(Exception ex) {
				

				var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
				toast.Show ();
				return;

			}


		}

		bool LoadFotos=false;

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
			return cursor.GetString(column_index);
		}

		public byte[] ReloadFoto( string imageUrl)
		{
			byte[] bitmapB = null;


			try{
				
				Android.Graphics.Bitmap bitmap;

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


			}catch(Exception ex) {
				throw ex;
			}
			return bitmapB;
		}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
			try
			{
			if (resultCode == Result.Ok) 
            {
				if (requestCode == 1) {
					try {
						Android.Net.Uri selectImage = data.Data;
						imageUrl1 = getRealPathFromURI (data.Data);
						imageurlId1 = data.Data.LastPathSegment;
						datetimedetalle = System.IO.File.GetLastAccessTimeUtc (imageUrl1);
						string fechahora = datetimedetalle.Day.ToString () + datetimedetalle.Month.ToString () + datetimedetalle.Year.ToString ().Substring (2, 2) + "-" + datetimedetalle.Hour.ToString () + datetimedetalle.Minute.ToString ();
							
						GC.Collect ();
						if (System.IO.File.Exists (imageUrl1)) {
							
							Android.Graphics.Bitmap bitmap = Utils.ChangeSizeBitmap (BitmapFactory.DecodeFile (imageUrl1), 1600, 1200);

							using (MemoryStream stream = new MemoryStream ()) {
								bitmap.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
								bitmapDetalle = stream.ToArray ();
								stream.Flush ();
								stream.Close ();
							}
								
							bitmap.Dispose ();
							if (bitmapDetalle == null) {
								Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
								return;
							} else {
								lblFotoDetalle.Text = ODTDet.Folio + " - Detalle - " + fechahora + ".jpg";
							}
						}
					} catch (Exception exc) {
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
						return;
					}

				} else if (requestCode == 2) {
					try {
						Android.Net.Uri selectImage = data.Data;
						imageUrl2 = getRealPathFromURI (data.Data);
						imageurlId2 = data.Data.LastPathSegment;
						datetimemediana = System.IO.File.GetLastAccessTimeUtc (imageUrl2);
						string fechahora = datetimemediana.Day.ToString () + datetimemediana.Month.ToString () + datetimemediana.Year.ToString ().Substring (2, 2) + "-" + datetimemediana.Hour.ToString () + datetimemediana.Minute.ToString ();
								
						GC.Collect ();

						if (System.IO.File.Exists (imageUrl2)) {

							Android.Graphics.Bitmap bitmap = Utils.ChangeSizeBitmap (BitmapFactory.DecodeFile (imageUrl2), 1600, 1200);

							using (MemoryStream stream = new MemoryStream ()) {
								bitmap.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
								bitmapMediana = stream.ToArray ();
								stream.Flush ();
								stream.Close ();
							}

							bitmap.Dispose ();
							if (bitmapMediana == null) {
								Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
								return;
							} else {
								lblFotoMediana.Text = ODTDet.Folio + " - Mediana - " + fechahora + ".jpg";
							}
									
						}
					} catch (Exception exc) {
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
						return;
					}
				} else if (requestCode == 3) {
					try {
						Android.Net.Uri selectImage = data.Data;
						imageUrl3 = getRealPathFromURI (data.Data);
						imageurlId3 = data.Data.LastPathSegment;
						datetimelarga = System.IO.File.GetLastAccessTimeUtc (imageUrl3);

						string fechahora = datetimelarga.Day.ToString () + datetimelarga.Month.ToString () + datetimelarga.Year.ToString ().Substring (2, 2) + "-" + datetimelarga.Hour.ToString () + datetimelarga.Minute.ToString ();

						GC.Collect ();
						if (System.IO.File.Exists (imageUrl3)) {

							Android.Graphics.Bitmap bitmap = Utils.ChangeSizeBitmap (BitmapFactory.DecodeFile (imageUrl3), 1600, 1200);

							using (MemoryStream stream = new MemoryStream ()) {
								bitmap.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
								bitmapLarga = stream.ToArray ();
								stream.Flush ();
								stream.Close ();
							}

							bitmap.Dispose ();
							if (bitmapLarga == null) {
								Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
								return;
							} else {
								lblFotoLarga.Text = ODTDet.Folio + " - Larga - " + fechahora + ".jpg";
							}
						}
					} catch (Exception exc) {
						Toast.MakeText (ApplicationContext, "No se cargó la fotografía correctamente, favor de seleccionarla de nuevo", ToastLength.Long).Show ();
						return;
					}
				} else if (requestCode == 4) {//extSdCard

					try{




						if (App._file.Exists ()) {


							if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat) {
								Intent mediaScanIntent = new Intent(
									Intent.ActionMediaScannerScanFile);
								var contentUri =  Android.Net.Uri.FromFile (App._file);
								mediaScanIntent.SetData(contentUri);
								this.SendBroadcast(mediaScanIntent);
							} else {
								//SendBroadcast(new Intent(Intent.ActionMediaMounted, MediaStore.Images.Thumbnails.ExternalContentUri));
								SendBroadcast(new Intent(Intent.ActionMediaMounted,Android.Net.Uri.Parse("file://"+ Android.OS.Environment.ExternalStorageDirectory)));
							}
								
						}

							//GC.Collect ();


					}catch(Exception ex) {
						var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
						toast.Show ();
					}

				} 



			}
			}catch(Exception ex) {
				var toast = Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long);
				toast.Show ();
			}
		}


		private void IniciarCamara( )
		{  
			try{
				if (IsThereAnAppToTakePictures()) 
				{
					CreateDirectoryForPictures ();

					Intent intent = new Intent (MediaStore.ActionImageCapture);
					App._file = new Java.IO.File (App._dir, ODTDet.Folio + DateTime.Now.Day.ToString () + DateTime.Now.Month.ToString () + DateTime.Now.Year.ToString () + "-" + DateTime.Now.Hour.ToString () + DateTime.Now.Minute.ToString () + DateTime.Now.Second + "_1.jpg");
					intent.PutExtra (MediaStore.ExtraOutput, Android.Net.Uri.FromFile (App._file));
					StartActivityForResult (intent, 4);

				}
			}catch(Exception ex) {
				throw ex;

			}
		}  


		private bool IsThereAnAppToTakePictures()
		{
			try{
				Intent intent = new Intent(MediaStore.ActionImageCapture);
				IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
				return availableActivities != null && availableActivities.Count > 0;
			}catch(Exception ex) {
				throw ex;
			}
		}


		private void CreateDirectoryForPictures()
		{
			try{
			App._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim), "Camera");

			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
			}catch(Exception ex) {
				throw ex;
			}
		}


	}
}



