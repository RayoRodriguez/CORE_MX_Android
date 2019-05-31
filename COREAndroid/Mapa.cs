using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;

using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Android.Locations;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Content;
using Android.Provider;

using Android.Views;
using Org.Json;
using Org.Apache.Http.Client.Methods;
using System.Net.Http;
using System.Text;
using Java.IO;
using Java.Net;
using System.Net;
using System.Collections.Generic;
using POL_CORE.core;
using Android.Net;
using SAO.Entity.Operation;
using Android.Graphics;



namespace POL_CORE
{
	[Activity (Label = "Mapa")]			
	public class Mapa :FragmentActivity, Android.Locations.ILocationListener
	{
		private LatLng currentLocation;
        private GoogleMap _map;

        private SupportMapFragment _mapFragment;
		private double lat;
		private double lng;
        
       // private GroundOverlay _polarBearOverlay;
        private LocationManager locationManager;
        private String provider;


		protected override void OnCreate (Bundle bundle)
		{
			try
			{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.Mapa);

			ImageButton ImgButton =  FindViewById<ImageButton> (Resource.Id.BtnRegresar);
			DTOODTCustom ODTDet= new DTOODTCustom ();
			ODTDet=(DTOODTCustom) Utils.Deserialize(typeof(DTOODTCustom),this.Intent.GetStringExtra ("ODT"));
			TextView lblODT = FindViewById<TextView> (Resource.Id.LblOrdenActual);
			lblODT.Text = ODTDet.Folio + " - " + ODTDet.NumeroVista;
			lat=this.Intent.GetDoubleExtra("lat",0);
			lng=this.Intent.GetDoubleExtra("lng",0);
			ImgButton.Click += delegate {
				Finish();
			};

			GetCurrentLocation ();
			InitMapFragment();
			SetupMapIfNeeded();

			}catch(Exception ex) {
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

			}

		}
		protected override void OnPause()
		{

			try
			{
				base.OnPause();
				if(locationManager!=null)
					locationManager.RemoveUpdates(this);
				if(_map!=null)
					_map.MyLocationEnabled = false;
			}catch(Exception ex) {
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}

		}


		protected override void OnStop()
		{

			try
			{
				base.OnStop();
				if(locationManager!=null)
					locationManager.RemoveUpdates(this);
				if(_map!=null)
					_map.MyLocationEnabled = false;
			}catch(Exception ex) {
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}



		}
		protected override void OnResume()
		{

			try{
				base.OnResume();
				if(locationManager!=null)
					locationManager.RequestLocationUpdates(provider, 400, 1, this);
				SetupMapIfNeeded();
				if(_map!=null)
					_map.MyLocationEnabled = true;

			}catch(Exception ex) {
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
		}


		private void InitMapFragment()
		{
			try{

				_mapFragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
				if (_mapFragment == null)
				{
					GoogleMapOptions mapOptions = new GoogleMapOptions()
						.InvokeMapType(GoogleMap.MapTypeNormal)
						.InvokeZoomControlsEnabled(true)
						.InvokeCompassEnabled(true);

					FragmentTransaction fragTx = SupportFragmentManager.BeginTransaction();
					_mapFragment = SupportMapFragment.NewInstance(mapOptions);
					fragTx.Add(Resource.Id.mapWithOverlay, _mapFragment, "map");
					fragTx.Commit();
				}
			}catch(Exception ex) {
				throw ex;
			}
		}


		private PolylineOptions JSONRuta()
		{
			PolylineOptions lineOptions = new PolylineOptions ();


				try 
				{
					JSONObject json = rutaEntreDosPuntos (getDirectionsUrl (currentLocation, new LatLng (lat, lng)));  
					
					if(json!=null)
					{
						JSONArray ruta = json.GetJSONArray ("routes").GetJSONObject (0).GetJSONArray ("legs").GetJSONObject (0).GetJSONArray ("steps");
						int numTramos = ruta.Length ();
						for (int i=0; i<numTramos; i++) 
						{
							JSONObject puntosCodificados = ruta.GetJSONObject (i).GetJSONObject ("start_location");
							lineOptions.Add (new LatLng (puntosCodificados.GetDouble ("lat"), puntosCodificados.GetDouble ("lng")));

							puntosCodificados = ruta.GetJSONObject (i).GetJSONObject ("polyline");
							List<LatLng> arr = decodePoly (puntosCodificados.GetString ("points"));

							foreach (LatLng pos in arr) {
								lineOptions.Add (pos);
							}
							puntosCodificados = ruta.GetJSONObject (i).GetJSONObject ("end_location");
							lineOptions.Add (new LatLng (puntosCodificados.GetDouble ("lat"), puntosCodificados.GetDouble ("lng")));

						}
					}
				}
				catch (JSONException e) 
				{
					throw e;

				}
				catch(Exception ex) 
				{
					throw ex;
				}
			return lineOptions;

		}







        private void SetupMapIfNeeded()
        {

			try
			{
		
				if (_map == null) 
				{
					_map = _mapFragment.Map;
					if (_map != null) 
					{
						_map.Clear ();
					if (currentLocation!=null)
						{

							MarkerOptions marker1 = new MarkerOptions ();
							marker1.InvokeIcon (BitmapDescriptorFactory.FromResource(Resource.Drawable.A));
							marker1.SetPosition(currentLocation);
							marker1.SetTitle ("A");
							_map.AddMarker (marker1);
						}


						MarkerOptions marker2 = new MarkerOptions ();
						marker2.SetPosition (new LatLng(lat,lng));
						marker2.InvokeIcon (BitmapDescriptorFactory.FromResource(Resource.Drawable.B));
						marker2.SetTitle ("B");
						_map.AddMarker (marker2);
						if (Utils.Internet(ApplicationContext)) 
						{
							if(currentLocation!=null  && currentLocation.Latitude!=0  && currentLocation.Longitude!=0  && lat != 0 && lng != 0)
							{ 
								var r=_map.AddPolyline (JSONRuta ());
								r.Color=Color.Red;


							}
							else
							{
								Toast.MakeText(this, "Se requiere conocer la ubicacion actual  y destino   para cargar la ruta", ToastLength.Long).Show();
							}
						} else 
						{
							Toast.MakeText(this, "Se requiere conexión a datos para cargar la ruta", ToastLength.Long).Show();

						}
					
						if (currentLocation != null) 
						{
							
							_map.AnimateCamera (CameraUpdateFactory.NewLatLngZoom (currentLocation, 13));
						}

					}
				}
			}catch(Exception ex) 
			{
				throw ex;
			}
		}
    


		private  List<LatLng> decodePoly(string encoded) 
		{
			List<LatLng> poly = new List<LatLng>();

			try
			{
				int index = 0, len = encoded.Length;
				int lat = 0, lng = 0;
				while (index < len) {
					int b, shift = 0, result = 0;
					do {
						b = encoded[index++] - 63;
						result |= (b & 0x1f) << shift;
						shift += 5;
					} while (b >= 0x20);
					int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
					lat += dlat;
					shift = 0;
					result = 0;
					do {
						b = encoded[index++] - 63;
						result |= (b & 0x1f) << shift;
						shift += 5;
					} while (b >= 0x20);
					int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
					lng += dlng;

					LatLng position = new LatLng((double) lat / 1E5, (double) lng / 1E5);
					poly.Add(position);
				}
			}catch(Exception ex) {
				throw ex;
			}
			return poly;
		}

		private JSONObject rutaEntreDosPuntos(string url)
		{
			JSONObject json= new JSONObject();

			try
			{
				HttpWebRequest oReq = (HttpWebRequest)WebRequest.Create(url);
				oReq.Method = "GET";
				ASCIIEncoding encoding = new ASCIIEncoding();
				HttpWebResponse resp = (HttpWebResponse)oReq.GetResponse();
				System.IO.StreamReader loResponseStream = new System.IO.StreamReader(resp.GetResponseStream(), encoding);
				String s = loResponseStream.ReadToEnd();
				loResponseStream.Close();
				json =  new JSONObject(s.Replace("&nbsp;", " "));

			}catch(Exception ex) {
				throw ex;
			}

			return json;
		}

		private string getDirectionsUrl(LatLng origin,LatLng dest)
		{
			
			string str_origin = "origin="+origin.Latitude.ToString().Replace(",",".")+","+origin.Longitude.ToString().Replace(",",".");
			string str_dest = "destination="+dest.Latitude.ToString().Replace(",",".")+","+dest.Longitude.ToString().Replace(",",".");  
			string sensor = "travelmode=driving&unitsystem=metric&region=mx&sensor=false";  
			string parameters = str_origin+"&"+str_dest+"&"+sensor;
			string url = "https://maps.googleapis.com/maps/api/directions/json?"+parameters; 
			return url;
		}


		private void GetCurrentLocation()
		{

			try
			{
				LocationManager service = (LocationManager)GetSystemService(LocationService);
				bool enabledGPS = service.IsProviderEnabled(LocationManager.GpsProvider);
				bool enabledWiFi = service.IsProviderEnabled(LocationManager.NetworkProvider);

				if (!enabledGPS) 
				{
					Intent intent = new Intent (Settings.ActionLocationSourceSettings);
					StartActivity (intent);
				}

				locationManager = (LocationManager)GetSystemService(Context.LocationService);
				Criteria criteria = new Criteria();
				criteria.Accuracy = Accuracy.Fine;
				criteria.AltitudeRequired = true;
				Location location = locationManager.GetLastKnownLocation("passive");
				location = locationManager.GetLastKnownLocation("passive");
				provider="passive";

				if (location != null)
				{

					currentLocation = new LatLng(location.Latitude, location.Longitude);
				}
				else
				{
					criteria.Accuracy = Accuracy.Fine;
					criteria.AltitudeRequired = true;
					location = locationManager.GetLastKnownLocation(locationManager.GetBestProvider(criteria,true));
					location = locationManager.GetLastKnownLocation(locationManager.GetBestProvider(criteria,true));
					provider = locationManager.GetBestProvider (criteria, true);
					if (location != null) 
					{
						currentLocation = new LatLng(location.Latitude, location.Longitude);
					} else 
					{
						location = locationManager.GetLastKnownLocation("gps");
						location = locationManager.GetLastKnownLocation("gps");
						provider = "gps";
						if (location != null)
						{

							currentLocation = new LatLng(location.Latitude, location.Longitude);
						} 

					}
				}

			}catch(Exception ex)
			{
				throw ex;
			}

		}



		public override bool OnKeyDown(Keycode keyCode, KeyEvent evento)  
		{
			base.OnKeyDown (keyCode, evento);

			if (keyCode==Keycode.Back) 
			{Finish ();

			}
			return true;

		}

	
		public void OnLocationChanged(Location location)
        {
			currentLocation = null;
            currentLocation = new LatLng(location.Latitude, location.Longitude);
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Enabled new provider " + provider, ToastLength.Long).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Disabled provider " + provider, ToastLength.Long).Show();
        }

		void Android.Locations.ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras)
		{
			
		 
		}

	
	}

}














