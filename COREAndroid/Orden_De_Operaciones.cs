using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;
using System.IO;
using Org.Json;
using POL_CORE.core;
using System.ComponentModel;
using Android.Provider;
using System.Threading.Tasks;
using SAO.Bss.Helper;
using SAO.Entity.Operation;

namespace POL_CORE
{
	[Activity (Label = "ORDEN DE OPERACIONES", MainLauncher = false)]			
	public class Orden_De_Operaciones : Activity
	{   
		

		private ProgressDialog _progressDialog;
		DTOODTEjecucion ExecGlobal=null;
		DTOODTCancelacion CancGlobal=null;
		DTOODTPospuesta PospGlobal=null;
		private DTOODTCustom[] ArrayODT;
		String[] color=
		{   "#FFFFFF",//blanco
			"#F5DC49",//Amarillo
			"#FE0101",//Rojo
			"#01FE1E",//Verde
			"#FF8000"// Anaranjado

		};
	

		protected override void OnCreate (Bundle bundle)
		{

			try
			{
				base.OnCreate (bundle);

				SetContentView (Resource.Layout.Orden_De_Operaciones);

				Button button = FindViewById<Button> (Resource.Id.btn_EndSession);
				Button Competencia = FindViewById<Button> (Resource.Id.btn_Competencia);

				button.Click += delegate 
				{
					CloseSession();
				};
				Competencia.Click += delegate 
				{
					var activityMain = new Intent (this, typeof(Competencia));
					StartActivityForResult(activityMain,1);
				};

				_progressDialog = new ProgressDialog(this) { Indeterminate = true };
				_progressDialog.SetMessage("Por favor espere...");
				_progressDialog.SetCanceledOnTouchOutside (false);

				_progressDialog.Show ();
				CargarInformacion ();
				RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
				GC.Collect ();
			}catch(Exception ex) {

				Toast.MakeText (ApplicationContext, ex.Message, ToastLength.Long).Show ();

			}


 
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent evento)  
		{
			base.OnKeyDown (keyCode, evento);

			if (keyCode==Keycode.Back) 
			{
				SetResult (Result.Ok);
				Finish ();

			}
			return true;

		}

		int ej=0,ca=0,po=0;


		async void bk_DoWorkE()
		{
			ej= await EjecucionMemoria.Finalizar (ExecGlobal, ApplicationContext);

			if (ej == 1) {
				//quitar de memoria Ejecucion
				EjecucionMemoria.QuitarMemoriaEjecucion (ExecGlobal.ODTID);
				EjecucionMemoria.QuitarMemoriaGlobal (ExecGlobal.ODTID);


				//quitar de memoria global

				_progressDialog.Hide ();
				Toast.MakeText (ApplicationContext, "La orden de trabajo se finalizó  correctamente", ToastLength.Long).Show ();
				_progressDialog.Show ();
				CargarInformacion ();
			} else 
			{
				_progressDialog.Hide ();
				Toast.MakeText (ApplicationContext, "La orden de trabajo no se finalizo", ToastLength.Long).Show ();
				return;

			}
		}



		async void bk_DoWorkP()
		{
			po = await EjecucionMemoria.Posponer (PospGlobal, ApplicationContext);
			if (po == 1) 
			{
				//quitar de memoria posponer
				EjecucionMemoria.QuitarMemoriaPosponer (PospGlobal.ODTID);
				EjecucionMemoria.QuitarMemoriaGlobal (PospGlobal.ODTID);


				//quitar de memoria global
				//quitar de memoria global
				if (ArrayODT != null && ArrayODT.Length > 0) {
					var i = ArrayODT.ToList ().FindIndex (x => x.ID == PospGlobal.ODTID);
					if (i != -1) {
						ArrayODT.ToList ().RemoveAt (i);
						await Load ();
					}
				}
				_progressDialog.Hide ();
				Toast.MakeText (ApplicationContext, "La ODT se pospuso correctamente", ToastLength.Long).Show();
				_progressDialog.Show ();
				CargarInformacion();

			}else 
			{
				_progressDialog.Hide ();
				Toast.MakeText (ApplicationContext, "La orden de trabajo no se finalizo", ToastLength.Long).Show ();
				return;

			}
		}



		async void bk_DoWorkC()
		{
			ca = await EjecucionMemoria.Cancelar (CancGlobal, ApplicationContext);
			if (ca == 1) 
			{
				//quitar de memoria cancelacion 
				EjecucionMemoria.QuitarMemoriaGlobal (CancGlobal.ODTID);
				EjecucionMemoria.QuitarMemoriaCancelacion (CancGlobal.ODTID);


				//quitar de memoria global
				if (ArrayODT != null && ArrayODT.Length > 0) {
					var i = ArrayODT.ToList ().FindIndex (x => x.ID == CancGlobal.ODTID);
					if (i != -1) {
						ArrayODT.ToList ().RemoveAt (i);
						await Load ();
					}
				}

				_progressDialog.Hide ();
				Toast.MakeText(ApplicationContext, "La orden de trabajo se canceló correctamente",ToastLength.Long).Show();

				_progressDialog.Show ();
				CargarInformacion();
			}
			else 
			{
				_progressDialog.Hide ();
				Toast.MakeText (ApplicationContext, "La orden de trabajo no se finalizo", ToastLength.Long).Show ();
				return;

			}
		}
		private void BKEjecucion(DTOODTEjecucion OE)
		{
			if (Utils.Internet (ApplicationContext)) {
				_progressDialog.Show ();
				ExecGlobal = OE;
				bk_DoWorkE();

			} else {
				Toast.MakeText(ApplicationContext, "No tienes acceso a internet para enviar la ODT.",ToastLength.Long).Show();
			}

		}
		private void BKPospones(DTOODTPospuesta OP)
		{	
			if (Utils.Internet (ApplicationContext)) {
				_progressDialog.Show ();
				PospGlobal = OP;
				bk_DoWorkP ();

			}else {
				Toast.MakeText(ApplicationContext, "No tienes acceso a internet para enviar la ODT.",ToastLength.Long).Show();
			}

		}

		private void BKCancelacion(DTOODTCancelacion OC)
		{
			if (Utils.Internet (ApplicationContext)) {
				_progressDialog.Show();
				CancGlobal = OC;
				bk_DoWorkC();

			}else {
				Toast.MakeText(ApplicationContext, "No tienes acceso a internet para enviar la ODT.",ToastLength.Long).Show();
			}
		}

		async Task<DTOODTCustom[]> LoadODTs()
		{
			DTOODTCustom[] Odts=null;

			return  await Task.Factory.StartNew (() =>
				{
					OperationClient WS=null;

					try{
						WS =Utils.InitializeServiceClient();
						Odts= WS.GetODTByCuadrilleroID (int.Parse (Utils.UsuarioApp.Iduser));

					}catch(Exception ex)
					{
						throw ex;
					}
					finally
					{
						if(WS!=null)
							WS.Close();
					}
					return Odts;
				});
		}

		async Task Load()
		{
			try{
				LinearLayout linearPadre = FindViewById<LinearLayout> (Resource.Id.Layout_OrdenTrabajo);
				linearPadre.RemoveAllViews ();

				LinkedList<DTOODTCustom> ListaODT = new LinkedList<DTOODTCustom> ();
				foreach (DTOODTCustom ODTDet in ArrayODT) 
				{
					DTOODTCustom Find = Utils.FindODT (ODTDet);
					LinearLayout LY = new LinearLayout (this);


					LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
						LinearLayout.LayoutParams.MatchParent);

					linearLayoutParams.SetMargins(0, 0, 0, 0);

					LY.LayoutParameters = linearLayoutParams;


					LY.Id = ODTDet.ID;
					LY.Orientation = Android.Widget.Orientation.Vertical;
					LY.SetGravity (GravityFlags.Center);



					TextView tvTitulo = new TextView (this);
					tvTitulo.Text = ODTDet.Folio + " - " + ODTDet.NumeroVista;
					tvTitulo.SetBackgroundResource (Resource.Drawable.ListViewOrden_Trabajo);
					tvTitulo.Gravity = Android.Views.GravityFlags.Center;
					if (Find == null) 
					{
						if (ODTDet.EsPospuesta == true) {tvTitulo.SetTextColor (Color.ParseColor (color [1]));}
						else {tvTitulo.SetTextColor (Color.ParseColor (color [0]));	}
						tvTitulo.Click += delegate 
						{
							var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
							activityMain.PutExtra ("ODT", Utils.Serialize (typeof(DTOODTCustom), ODTDet));
							StartActivityForResult (activityMain, 1);
						};
						ListaODT.AddLast (ODTDet);
					} else 
					{
						if (Find.EstatusID == 114) 
						{
							DTOODTEjecucion Exec= EjecucionMemoria.FindEjecucion (Find);
							if (Exec != null) 
							{
								tvTitulo.SetTextColor (Color.ParseColor (color [3]));
								tvTitulo.Click += delegate {BKEjecucion (Exec);};
								ListaODT.AddLast (Find);
							} else 
							{
								if (ODTDet.EsPospuesta == true) 
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [1]));
								} else 
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [0]));
								}
								tvTitulo.Click += delegate 
								{
									var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
									activityMain.PutExtra ("ODT", Utils.Serialize (typeof(DTOODTCustom), ODTDet));
									StartActivityForResult (activityMain, 1);
								};
								ListaODT.AddLast (ODTDet);
							}
						} else if (Find.EstatusID == 113)
						{

							DTOODTPospuesta pos= EjecucionMemoria.FindPospuesta (Find);
							if (pos != null) 
							{													
								tvTitulo.SetTextColor (Color.ParseColor (color [1]));
								tvTitulo.Click += delegate {BKPospones (pos);};
								ListaODT.AddLast (Find);
							} else 
							{ 
								if (ODTDet.EsPospuesta == true) 
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [1]));
								} else
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [0]));
								}
								tvTitulo.Click += delegate
								{
									var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
									activityMain.PutExtra ("ODT", Utils.Serialize (typeof(DTOODTCustom), ODTDet));
									StartActivityForResult (activityMain, 1);
								};
								ListaODT.AddLast (ODTDet);
							}
						} else if (Find.EstatusID == 117)
						{
							DTOODTCancelacion can = EjecucionMemoria.FindCancelacion (Find);
							if (can != null) 
							{
								tvTitulo.SetTextColor (Color.ParseColor (color [2]));
								tvTitulo.Click += delegate {
									BKCancelacion(can);
								};
								ListaODT.AddLast (Find);
							} else 
							{
								if (ODTDet.EsPospuesta == true) 
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [1]));
								} else 
								{
									tvTitulo.SetTextColor (Color.ParseColor (color [0]));
								}
								tvTitulo.Click += delegate
								{
									var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
									activityMain.PutExtra ("ODT", Utils.Serialize (typeof(DTOODTCustom), ODTDet));
									StartActivityForResult (activityMain, 1);
								};	
								ListaODT.AddLast (ODTDet);								
							}

						}

					}

					tvTitulo.Clickable = true;
					LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
						LinearLayout.LayoutParams.MatchParent);

					p.SetMargins(10, 0, 15, 20);
					tvTitulo.LayoutParameters = p;
					LY.AddView (tvTitulo);
					linearPadre.AddView (LY);

				}
				Utils.Memoria = ListaODT.ToArray<DTOODTCustom>();

			}catch(Exception ex) {
				throw ex;
			}
		}

		private async void CargarInformacion()
		{

			try
			{
				if (Utils.Internet (ApplicationContext)) 
				{
					
					try
					{
						await Task.Factory.StartNew (async () =>
							{
								await Utils.EjecutarMemoria (ApplicationContext);
							});

						
						ArrayODT = await LoadODTs();

						if (ArrayODT == null) 
						{
							if (Utils.Memoria != null && Utils.Memoria.Length>0) {
								await CargarMemoria ();
							} else 
							{
								Toast.MakeText(this, "No hay ninguna ODT disponible.", ToastLength.Long).Show();
							
							}
						} 
						else 
						{
							if (ArrayODT.Length > 0) {
								await Load ();
							} else {
								
								Toast.MakeText (this, "No hay ninguna ODT disponible", ToastLength.Long).Show ();

							}
						}

						this.RunOnUiThread(delegate {
							_progressDialog.Hide ();
						});



					}
					catch( Exception exc)
					{
						_progressDialog.Hide ();
						Toast.MakeText (this, exc.Message, ToastLength.Long).Show ();
						return;

					}


				} else
				{
					try{
						if (Utils.Memoria != null && Utils.Memoria.Length>0) {
							await CargarMemoria ();
						} else 
						{
							Toast.MakeText(this, "No hay ninguna ODT disponible.", ToastLength.Long).Show();
						
						}
						_progressDialog.Hide ();

					}catch(Exception ex) {

						_progressDialog.Hide ();
						Toast.MakeText (this, ex.Message, ToastLength.Long).Show ();
						return;
					}

				}

			}catch(Exception ex) {
				throw ex;
			}
		}

		private async Task CargarMemoria()
		{

			try{
				DTOODTCustom[] ArrayODTMem = Utils.Memoria;
				LinearLayout linearPadre = FindViewById<LinearLayout>(Resource.Id.Layout_OrdenTrabajo);
				linearPadre.RemoveAllViews ();
				

				foreach (DTOODTCustom ODTDet in ArrayODTMem) 
				{

					LinearLayout LY = new LinearLayout (this);
					LY.Id = ODTDet.ID;
					LY.Orientation = Android.Widget.Orientation.Vertical;
					LY.SetGravity (GravityFlags.Center);
					TextView tvTitulo = new TextView (this);
					tvTitulo.Text =ODTDet.Folio + " - " + ODTDet.NumeroVista;
					tvTitulo.SetBackgroundResource (Resource.Drawable.ListViewOrden_Trabajo);
					tvTitulo.Gravity = Android.Views.GravityFlags.Center;


					switch (ODTDet.EstatusID) 
					{
						case 113://programada--blanco
						tvTitulo.SetTextColor(Color.ParseColor(color[0])) ;
						break;
						case 114://Ejecutada-Verde
						tvTitulo.SetTextColor(Color.ParseColor(color[3])) ;
						break;
						case 115://Ejecutada-Verde
						tvTitulo.SetTextColor(Color.ParseColor(color[4])) ;
						break;
						case 116://Ejecutada-Verde
						tvTitulo.SetTextColor(Color.ParseColor(color[3])) ;
						break;
						case 117://Cancelada --Rojo
						tvTitulo.SetTextColor (Color.ParseColor (color [2]));
						break;

					}
					if ( ODTDet.EsPospuesta==true && ODTDet.EstatusID==113)
					{
						tvTitulo.SetTextColor (Color.ParseColor (color [1]));
					} 

					tvTitulo.Click += delegate 
					{

						//primero preguntar si hay internet
						//despues buscar la odt si se encuentra en memoria de ejecucion, cancelacion, pospuesta.
						//al dar click que se haga el envio de la odt.
						if (Utils.Internet (ApplicationContext)) //Hay internet
						{
							if(ODTDet.EstatusID==113 && ODTDet.EsPospuesta==false)
							{
								var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
								string alan = ODTDet.ToString();
								activityMain.PutExtra ("ODT", Utils.Serialize(typeof(DTOODTCustom),ODTDet));
								StartActivityForResult(activityMain,1);
							} else if (ODTDet.EstatusID==113 && ODTDet.EsPospuesta==true)
							{
								DTOODTPospuesta pos= EjecucionMemoria.FindPospuesta (ODTDet);
								if (pos != null) 
								{													
									BKPospones (pos);
								} else 
								{ 
									var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
									string alan = ODTDet.ToString();
									activityMain.PutExtra ("ODT", Utils.Serialize(typeof(DTOODTCustom),ODTDet));
									StartActivityForResult(activityMain,1);
								}

							}
							else if(ODTDet.EstatusID==114)
							{
								DTOODTEjecucion Exec= EjecucionMemoria.FindEjecucion (ODTDet);
								if (Exec != null) 
								{
									BKEjecucion (Exec);

								} else 
								{
									Toast.MakeText(this, "No cuentas con internet para el envio de ODT, solo se pueden modificar ODT programadas", ToastLength.Long).Show();
									return;

								}
							} else if(ODTDet.EstatusID==117)
							{
								DTOODTCancelacion can = EjecucionMemoria.FindCancelacion (ODTDet);
								if (can != null) 
								{
										BKCancelacion(can);
								} else 
								{
									Toast.MakeText(this, "No cuentas con internet para el envio de ODT, solo se pueden modificar ODT programadas", ToastLength.Long).Show();
									return;
															
								}
							}


						}else //No hay internet
						{
							if(ODTDet.EstatusID==113 ||( ODTDet.EstatusID==113 && ODTDet.EsPospuesta==true))
							{
								var activityMain = new Intent (this, typeof(Orden_De_Trabajo));
								string alan = ODTDet.ToString();
								activityMain.PutExtra ("ODT", Utils.Serialize(typeof(DTOODTCustom),ODTDet));
								StartActivityForResult(activityMain,1);
							} 
							else 
							{
								Toast.MakeText(this, "No cuentas con internet para el envio de ODT, solo se pueden modificar ODT programadas", ToastLength.Long).Show();
								return;

							}

						}

					};
					tvTitulo.Clickable = true;
					LY.AddView (tvTitulo, 410, 70);
					linearPadre.AddView (LY, 410, 80);

				}

			}catch(Exception ex) {
				throw ex;
			}

		}


		private void CloseSession()
		{
			try
			{
				Utils.UsuarioApp = null;
				Utils.MemoriaCancelacion = null;
				Utils.MemoriaPosponer = null;
				Utils.MemoriaEjecucion = null;
				Utils.Memoria = null;

				var activityMain = new Intent(this,typeof(MainActivity));
				StartActivity(activityMain);
				Finish ();
			}catch(Exception ex) {
				throw ex;
			}
		}

	
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok)
			{

				SetResult(Result.Ok);
				_progressDialog.Show ();
				CargarInformacion ();

			}

		}
	}
}

