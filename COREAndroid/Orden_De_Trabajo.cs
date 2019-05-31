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
using POL_CORE.core;
using SAO.Entity.Operation;

namespace POL_CORE
{
	[Activity (Label = "Orden_De_Trabajo")]			
	public class Orden_De_Trabajo : Activity
	{
		private string ODT="";
		private double lat;
		private double lng;


		protected override void OnCreate (Bundle bundle)
		{

			try
			{
				base.OnCreate (bundle);
				RequestWindowFeature (WindowFeatures.NoTitle);
				SetContentView (Resource.Layout.Orden_De_Trabajo);

				Button button = FindViewById<Button> (Resource.Id.BtnVerMapa);
				ImageButton ImgButton =  FindViewById<ImageButton> (Resource.Id.BtnRegresar);
				Button btnIniciar = FindViewById <Button> (Resource.Id.BtnIniciar); 
				Button btnPosponer = FindViewById <Button> (Resource.Id.BtnPosponer);
				Button btnPreCancelar = FindViewById <Button> (Resource.Id.BtnCancelar); 
				LinearLayout lyPosp = FindViewById<LinearLayout> (Resource.Id.linearLayoutBtnPosponer);
				lyPosp.Visibility = ViewStates.Gone;


				btnIniciar.Click+=delegate {Iniciar();};
				button.Click += delegate {Mapa();};
				ImgButton.Click += delegate 
				{   
					SetResult(Result.Ok);
					Finish();
				};

				btnPosponer.Click += delegate {Posponer();};
				btnPreCancelar.Click += delegate {Cancelar();};
			
				if (bundle != null) {

					if (bundle.ContainsKey ("ODT")) {
						ODT = bundle.GetString ("ODT");
						if(!string.IsNullOrEmpty(ODT))
							ODTDet=(DTOODTCustom) Utils.Deserialize(typeof(DTOODTCustom),ODT);
					}
				} else {
					LoadODTIntent ();
				}

				CargarODT ();
				GC.Collect ();
			}catch(Exception ex) {

				Toast.MakeText (this, ex.Message, ToastLength.Long).Show ();

			}
		}

		void LoadODTIntent()
		{
			try
			{
				ODT = this.Intent.GetStringExtra ("ODT");
				ODTDet=(DTOODTCustom) Utils.Deserialize(typeof(DTOODTCustom),ODT);
			}catch(Exception ex) {
				throw ex;
			}

		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			try
			{
			if(!string.IsNullOrEmpty(ODT))
				outState.PutString ("ODT", ODT);
			}catch(Exception ex) {
				Toast.MakeText (this, ex.Message, ToastLength.Long).Show ();
			}

		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent evento)  
		{
			base.OnKeyDown (keyCode, evento);

			if (keyCode==Keycode.Back) 
			{
				SetResult(Result.Ok);
				Finish ();
			}
			return true;
		}

		DTOODTCustom ODTDet= new DTOODTCustom ();

		private void CargarODT()
		{
			try
			{
						TextView lblactividad = FindViewById <TextView> (Resource.Id.LblActividadData);
						TextView lblvistaTitulo= FindViewById <TextView> (Resource.Id.LblNumeroDeVista);
						TextView lblvista= FindViewById <TextView> (Resource.Id.LblNumeroDeVistaData);
						TextView lbldireccionTitulo= FindViewById <TextView> (Resource.Id.LblDireccion);
						TextView lbldireccion= FindViewById <TextView> (Resource.Id.LblDireccionData);
						TextView lblciudadtitulo=FindViewById <TextView> (Resource.Id.LblCiudad);
						TextView lblciudad=FindViewById <TextView> (Resource.Id.LblCiudadData);
						TextView lblmedidasTitulo=FindViewById <TextView> (Resource.Id.LblMedidas);
						TextView lblmedidas=FindViewById <TextView> (Resource.Id.LblMedidasData);

						TextView lblversion2= FindViewById <TextView> (Resource.Id.LblVersion);
						TextView lblversion= FindViewById <TextView> (Resource.Id.LblVersionData);
						TextView lbltitle = FindViewById<TextView> (Resource.Id.LblTituloPantalla);
						TextView lblLuz2 = FindViewById<TextView> (Resource.Id.LblLuz);
						TextView lblLuz = FindViewById<TextView> (Resource.Id.LblLuzData);
						TextView lblvistareubicacion = FindViewById<TextView> (Resource.Id.LblNumeroDeVistareubicacion);
						TextView lblvistareubicaciondata = FindViewById<TextView> (Resource.Id.LblNumeroDeVistareubicacionData);
						TextView lbldireccionreubicacion = FindViewById<TextView> (Resource.Id.LblDireccionreubicacion);
						TextView lbldireccionreubicaciondata = FindViewById<TextView> (Resource.Id.LblDireccionreubicacionData);
						TextView lblciudaddestino = FindViewById<TextView> (Resource.Id.LblCiudadDestino);
						TextView lblciudaddestinodata = FindViewById<TextView> (Resource.Id.LblCiudadDestinoData);
						TextView lblmedidasdestino = FindViewById<TextView> (Resource.Id.LblMedidasDestino);
						TextView lblmedidasdestinodata = FindViewById<TextView> (Resource.Id.LblMedidasDestinoData);
						TextView LblAdicionales =FindViewById <TextView> (Resource.Id.LblAdicionales);
						TextView LblAdicionalesdata =FindViewById <TextView> (Resource.Id.LblAdicionalesData);


						lbltitle.Text = "      ODT - " + ODTDet.Folio;


						//unidades móviles
						if(ODTDet.TipoTrabajoID==3)
						{
							

							lblactividad.Text = ODTDet.TipoODT;
							lblvistaTitulo.Text="Nombre ruta";
							lblvista.Text=ODTDet.NombreUnidadMovil;
							lbldireccionTitulo.Text="Ciudad";
							lbldireccion.Text=ODTDet.CiudadUnidadMovil;
							
							lblmedidasTitulo.Visibility = ViewStates.Gone;
							lblmedidas.Visibility = ViewStates.Gone;
							lblversion2.Visibility = ViewStates.Gone;
							lblversion.Visibility = ViewStates.Gone;
							lblLuz2.Visibility = ViewStates.Gone;
							lblLuz.Visibility = ViewStates.Gone;
							lblvistareubicacion.Visibility = ViewStates.Gone;
							lblvistareubicaciondata.Visibility = ViewStates.Gone;
							lbldireccionreubicacion.Visibility = ViewStates.Gone;
							lbldireccionreubicaciondata .Visibility = ViewStates.Gone;
							lblciudaddestino.Visibility = ViewStates.Gone;
							lblciudaddestinodata .Visibility = ViewStates.Gone;
							lblmedidasdestino.Visibility = ViewStates.Gone;
							lblmedidasdestinodata.Visibility = ViewStates.Gone;
							LblAdicionalesdata.Text = String.IsNullOrEmpty (ODTDet.InstruccionesAdicionales) ? "" : ODTDet.InstruccionesAdicionales;

							//Valla móvil
							if(ODTDet.TipoServicioID==103)
							{
								lblciudad.Text="Zona de exhibición";
								lblciudad.Text= ODTDet.ZonaExhibicion;
								
						
							}

							//Taxi
							if(ODTDet.TipoServicioID==120)
							{
								lblciudad.Text="Tipo unidad";
								lblciudad.Text= ODTDet.TipoUnidad;
							}

							//Autobus
							if(ODTDet.TipoServicioID==169)
							{
								lblciudad.Text="Recorrido completo";
								lblciudad.Text= ODTDet.Recorrido;
							}
							


						}else
						{
							lblactividad.Text = ODTDet.TipoODT;
							lblvista.Text = ODTDet.NumeroVista;
							lbldireccion.Text = ODTDet.DireccionCompleta;
							lblciudad.Text = ODTDet.Ciudad;
							lblmedidas.Text = ODTDet.Medidas;
							lblversion.Text = ODTDet.VersionNueva;

							if (ODTDet.TipoODT.Equals ("Reubicación")) 
							{
								lblvistareubicacion.Visibility = ViewStates.Visible;
								lbldireccionreubicacion.Visibility = ViewStates.Visible;
								lblvistareubicaciondata.Visibility = ViewStates.Visible;
								lbldireccionreubicaciondata.Visibility = ViewStates.Visible;
								lblciudaddestino.Visibility = ViewStates.Visible;
								lblmedidasdestino.Visibility = ViewStates.Visible;
								lblvistareubicaciondata.Text = ODTDet.NumeroVistaReubicacion;
								lbldireccionreubicaciondata.Text = ODTDet.DireccionCompletaReubicacion;
								lblciudaddestinodata.Text = ODTDet.CiudadReubicacion;
								lblmedidasdestinodata.Text = ODTDet.MedidasReubicacion;
								lat = Convert.ToDouble (ODTDet.LatitudVistaReubicacion);
								lng = Convert.ToDouble (ODTDet.LongitudVistaReubicacion);

								if (ODTDet.TipoODTID == 7 || ODTDet.TipoODTID == 8 || ODTDet.TipoODTID == 9 || ODTDet.TipoODTID == 10)
								{
									lblversion.Visibility = ViewStates.Gone;
									lblversion2.Visibility = ViewStates.Gone;
									lblLuz.Visibility = ViewStates.Gone;
									lblLuz2.Visibility = ViewStates.Gone;

								} 

							} else {


								lblvistareubicacion.Visibility = ViewStates.Gone;
								lbldireccionreubicacion.Visibility = ViewStates.Gone;
								lblvistareubicaciondata.Visibility = ViewStates.Gone;
								lbldireccionreubicaciondata.Visibility = ViewStates.Gone;
								lblciudaddestino.Visibility = ViewStates.Gone;
								lblciudaddestinodata.Visibility = ViewStates.Gone;
								lblmedidasdestino.Visibility = ViewStates.Gone;
								lblmedidasdestinodata.Visibility = ViewStates.Gone;
								lat = Convert.ToDouble (ODTDet.LatitudVista);
								lng = Convert.ToDouble (ODTDet.LongitudVista);

								if (ODTDet.TipoODTID == 7 || ODTDet.TipoODTID == 8 || ODTDet.TipoODTID == 9 || ODTDet.TipoODTID == 10)
								{
									lblversion.Visibility = ViewStates.Gone;
									lblversion2.Visibility = ViewStates.Gone;
									lblLuz.Visibility = ViewStates.Gone;
									lblLuz2.Visibility = ViewStates.Gone;

								}

							}

							if(ODTDet.EsConIluminacion)
							{
								lblLuz.Text="Si";
							}else
							{
								lblLuz.Text="No";
							}

							LblAdicionalesdata.Text = String.IsNullOrEmpty (ODTDet.InstruccionesAdicionales) ? "" : ODTDet.InstruccionesAdicionales;

							}

						

			}catch(Exception ex) {

				throw ex;
			}
		}
	

		private void Mapa()
		{
			try
			{
				if (ODTDet.NumeroVista != null) {
					var activityMain = new Intent(this,typeof(Mapa));
					activityMain.PutExtra ("ODT", ODT);
					activityMain.PutExtra ("lat", lat);
					activityMain.PutExtra ("lng", lng);
					StartActivityForResult (activityMain, 1);
				} else 
				{
					Toast.MakeText (this, "La orden de trabajo no cuenta con una vista,no se puede observar el mapa", ToastLength.Long).Show ();
				}
			}catch(Exception ex) {
				throw ex;
			}
		}

		private void Iniciar()
		{
			try
			{
				var activityMain = new Intent(this,typeof(Finalizar_Orden_De_Trabajo));
				activityMain.PutExtra("ODT",ODT);
				StartActivityForResult(activityMain,1);

			}catch(Exception ex) {
				throw ex;
			}
		
		}

		private void Posponer()
		{
			try
			{
				var activityMain = new Intent(this,typeof(Posponer_Orden_De_Trabajo));
				activityMain.PutExtra("POSPONER_ODT",ODT);
				StartActivityForResult(activityMain,2);
			}catch(Exception ex) {
				throw ex;
			}
			
		}

		private void Cancelar()
		{
			try
			{
				var activityMain = new Intent(this,typeof(Cancelar_Orden_De_Trabajo));
				activityMain.PutExtra("CANCELAR_ODT",ODT);
				StartActivityForResult(activityMain,3);
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
				Finish();
			}

		}



	}
}

