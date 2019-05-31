using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net.Http;
using POL_CORE.core;
using Android.Widget;
using Android.Provider;
using System.Threading.Tasks;
using System.ServiceModel;
using SAO.Entity.Operation;
using SAO.Bss.Helper;

namespace POL_CORE
{
	public static class Utils
	{
		
		public static readonly EndpointAddress EndPoint = new EndpointAddress("http://207.248.59.130:2874/SAO.Services_SandBox/Services/Operation.svc");

		public static OperationClient InitializeServiceClient()
		{
			BasicHttpBinding binding = CreateBasicHttp();

			return new OperationClient(binding, EndPoint);

		}

		private static BasicHttpBinding CreateBasicHttp()
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};
			TimeSpan timeout = new TimeSpan(0, 8, 30);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;
			return binding;
		}

		public static void Sort<TSource, TValue>(this List<TSource> source, Func<TSource, TValue> selector)
		{
			var comparer = Comparer<TValue>.Default;
			source.Sort((x, y) => comparer.Compare(selector(x), selector(y)));
		}

		public static string compress(string text)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			MemoryStream ms = new MemoryStream();
			using (System.IO.Compression.GZipStream zip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
			{
				zip.Write(buffer, 0, buffer.Length);
			}

			ms.Position = 0;
			MemoryStream outStream = new MemoryStream();

			byte[] compressed = new byte[ms.Length];
			ms.Read(compressed, 0, compressed.Length);

			byte[] gzBuffer = new byte[compressed.Length + 4];
			System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
			System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
			return Convert.ToBase64String(gzBuffer);
		}


		public static string Serialize(Type typeToSerialize, object objectToSerialize)
		{
			var serializer = new XmlSerializer (typeToSerialize);


			var sb = new StringBuilder();

			using (TextWriter writer = new StringWriter(sb))
			{
				serializer.Serialize(writer, objectToSerialize);
			}

			return sb.ToString();
		}




		public static object Deserialize(Type typeToDeserialize, string xmlString)
		{

			try
			{
				var serializer = new XmlSerializer(typeToDeserialize);
				object result;

				using (TextReader reader = new StringReader(xmlString))
				{
					result = serializer.Deserialize(reader);
				}

				return result;
			}
			catch (Exception ex)
			{

				throw ex;
			}

		}

		public static bool Internet(Context app)
		{
			Context context = app;
			Android.Net.ConnectivityManager connectMgr = (Android.Net.ConnectivityManager)context.GetSystemService (Context.ConnectivityService);
			if (connectMgr != null) {
				Android.Net.NetworkInfo[] netInfo = connectMgr.GetAllNetworkInfo ();
				if (netInfo != null) 
				{
					foreach (Android.Net.NetworkInfo net in netInfo) 
					{
						if (net.GetState () == Android.Net.NetworkInfo.State.Connected) 
						{
							if (net.Reason != null) 
							{
									return true;

							} else if (net.TypeName.Equals ("WIFI"))
							{
								return true;
							}
						}
					}
				} 

			}
			return false;
		}
		public static Usuario UsuarioApp
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("UsuarioApp", FileCreationMode.Append);
				var usuarioSerializado = prefs.GetString("Usuario", "");
				var usuario = usuarioSerializado == "" ? null : (Usuario)Utils.Deserialize(typeof(Usuario), usuarioSerializado);
				return usuario;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("UsuarioApp", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("Usuario", Utils.Serialize(typeof(Usuario),value));
				}
				prefEditor.Commit();

			}
		}



		public static DTOODTCustom[] Memoria
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("Memoria", FileCreationMode.Append);
				var memoriaSerializado = prefs.GetString("Memoria", "");
				var memoria = memoriaSerializado == "" ? null : (DTOODTCustom[])Utils.Deserialize(typeof(DTOODTCustom[]), memoriaSerializado);
				return memoria;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("Memoria", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("Memoria", Utils.Serialize( typeof(DTOODTCustom[]),value));
				}
				prefEditor.Commit();

			}
		}

		public static List<DTOODTEjecucion> MemoriaEjecucion
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaEjecucion", FileCreationMode.Append);
				var memoriaSerializado = prefs.GetString("MemoriaEjecucion", "");
				var memoria = memoriaSerializado == "" ? null : (List<DTOODTEjecucion> )Utils.Deserialize(typeof(List<DTOODTEjecucion>), memoriaSerializado);
				return memoria;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaEjecucion", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("MemoriaEjecucion", Utils.Serialize(typeof(List<DTOODTEjecucion>),value));
				}
				prefEditor.Commit();

			}

		}
		public static List<DTOODTCancelacion> MemoriaCancelacion
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaCancelacion", FileCreationMode.Append);
				var memoriaSerializado = prefs.GetString("MemoriaCancelacion", "");
				var memoria = memoriaSerializado == "" ? null : (List<DTOODTCancelacion>)Utils.Deserialize(typeof(List<DTOODTCancelacion>), memoriaSerializado);
				return memoria;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaCancelacion", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("MemoriaCancelacion", Utils.Serialize(typeof(List<DTOODTCancelacion>),value));
				}
				prefEditor.Commit();

			}

		}
		public static List<DTOODTPospuesta> MemoriaPosponer
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaPosponer", FileCreationMode.Append);
				var memoriaSerializado = prefs.GetString("MemoriaPosponer", "");
				var memoria = memoriaSerializado == "" ? null : (List<DTOODTPospuesta> )Utils.Deserialize(typeof(List<DTOODTPospuesta>), memoriaSerializado);
				return memoria;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaPosponer", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("MemoriaPosponer", Utils.Serialize(typeof(List<DTOODTPospuesta>),value));
				}
				prefEditor.Commit();

			}

		}

		public static List<Fotos> FotosMemoria
		{
			get
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaFotos", FileCreationMode.Append);
				var memoriaSerializado = prefs.GetString("MemoriaFotos", "");
				var memoria = memoriaSerializado == "" ? null : (List<Fotos> )Utils.Deserialize(typeof(List<Fotos>), memoriaSerializado);
				return memoria;
			}
			set
			{
				var prefs = Application.Context.GetSharedPreferences("MemoriaFotos", FileCreationMode.Append);
				var prefEditor = prefs.Edit();
				prefEditor.Clear();
				if (value != null)
				{
					prefEditor.PutString("MemoriaFotos", Utils.Serialize(typeof(List<Fotos>),value));
				}
				prefEditor.Commit();

			}

		}
		public static int getVersionCode(Context context)
		{
			PackageManager pm = context.PackageManager;
			try
			{
				PackageInfo pi = pm.GetPackageInfo(context.PackageName, 0);
				return pi.VersionCode;
			}
			catch (Android.Content.PM.PackageManager.NameNotFoundException ex) {string ea = ex.ToString ();}
			return 0;
		}

		public static void BorrarMemoriaPospuesta(DTOODTCustom odt)
		{
		List<DTOODTPospuesta> Mem = Utils.MemoriaPosponer;
			if (Mem!= null) 
			{
				int index = 0;
				foreach (DTOODTPospuesta od in Mem) 
				{
					if (od.ODTID == odt.ID) 
					{
						break;
					}
					index++;
				}

				Mem.RemoveAt (index);
				Utils.MemoriaPosponer = Mem;
			}


		}

		public static void  ActualizarMemoria(DTOODTCustom odt,short estatus) 
		{
			DTOODTCustom[] Mem = Utils.Memoria;
			if (Mem!= null) 
			{
				int index = 0;
				foreach (DTOODTCustom od in Mem) 
				{
					if (od.ID == odt.ID) 
					{
						break;
					}
					index++;
				}

				Mem[index].EstatusID = estatus;
				if (estatus == 113)
					Mem [index].EsPospuesta = true;
				Utils.Memoria = Mem;
			}




		}

		public static DTOODTCustom FindODT(DTOODTCustom odt)
		{
			DTOODTCustom[] Mem = Utils.Memoria;
			DTOODTCustom ODTFind = null;
			if (Mem != null) {
				foreach (DTOODTCustom od in Mem) {
					if (od.ID == odt.ID && (od.EstatusID == 114 || od.EstatusID == 117 || (od.EstatusID == 113&&od.EsPospuesta==true))) {
						ODTFind = od;
						break;
					}

				}
			}

			return ODTFind;

		}

		public static async Task<int> CargarFoto(string folio,byte[] Foto,Context ApplicationContext,string fecha)
		{
			int retorno=0;

			return  await Task.Factory.StartNew (() =>
				{
					OperationClient WS=null;
					try{
						WS=Utils.InitializeServiceClient();
						OperationStatus Request=WS.UploadODTFotoForMovil(folio,Utils.compress(Convert.ToBase64String(Foto,Base64FormattingOptions.InsertLineBreaks)));
						if (Request.IsComplete) 
						{
							retorno= Convert.ToInt32 (Request.RecordId);

						}

					}catch(Exception ex)
					{
						retorno=0;
					}
					finally
					{
						if(WS!=null)
							WS.Close();
					}
					return retorno;
				});
			
		}



		public static void EliminarFotoMemoria(string odt,ContentResolver _resolver)
		{
			List<Fotos> Mem = Utils.FotosMemoria;
			if (Mem != null) 
			{
				foreach (Fotos f in Mem) 
				{
					if (f.IdODT == odt) {
						if (System.IO.File.Exists (f.Url)) 
						{
							int i = _resolver.Delete (MediaStore.Images.Thumbnails.ExternalContentUri, MediaStore.Images.Thumbnails.ImageId + " = ?", new String[] { "" + f.UrlId});
							Application.Context.ContentResolver.Delete(MediaStore.Images.Media.ExternalContentUri, BaseColumns.Id + "=" + f.UrlId, null);
							if (i > 0)
							{
								System.IO.File.Delete (f.Url);
								Mem.Remove (f);
							}
						}
					}

				}

				Utils.FotosMemoria = Mem;
			}

		}

		public static async Task EjecutarMemoria(Context ApplicationContext )
		{
			try
			{
				List<DTOODTEjecucion> ejecucion = Utils.MemoriaEjecucion;
				List<DTOODTCancelacion> cancelacion = Utils.MemoriaCancelacion;
				List<DTOODTPospuesta> pospone = Utils.MemoriaPosponer;
				OperationClient WS=null;

				if (ejecucion != null)
				{
					List<DTOODTEjecucion> NoExec = new List<DTOODTEjecucion> ();
					int CountExec = 0;
					foreach (DTOODTEjecucion insert in ejecucion) 
					{

						try {

							int detalle = await Utils.CargarFoto (insert.ODTID.ToString (), insert.CortaFoto.Foto,ApplicationContext,insert.CortaFoto.FechaCreacion.Day.ToString()+"/"+insert.CortaFoto.FechaCreacion.Month.ToString()+"/"+insert.CortaFoto.FechaCreacion.Year.ToString());
							int mediana = await Utils.CargarFoto (insert.ODTID.ToString (), insert.MediaFoto.Foto,ApplicationContext,insert.MediaFoto.FechaCreacion.Day.ToString()+"/"+insert.MediaFoto.FechaCreacion.Month.ToString()+"/"+insert.MediaFoto.FechaCreacion.Year.ToString());
							int larga = await Utils.CargarFoto (insert.ODTID.ToString (), insert.LargaFoto.Foto,ApplicationContext,insert.LargaFoto.FechaCreacion.Day.ToString()+"/"+insert.LargaFoto.FechaCreacion.Month.ToString()+"/"+insert.LargaFoto.FechaCreacion.Year.ToString());

							if(detalle!=0 && mediana!=0 && larga!=0)
							{
								insert.CortaFotoID = detalle;
								insert.MediaFotoID = mediana;
								insert.LargaFotoID = larga;
								insert.CortaFoto.Foto=null;
								insert.MediaFoto.Foto=null;
								insert.LargaFoto.Foto=null;
								WS=Utils.InitializeServiceClient();
								OperationStatus Request =  WS.InsertODTEjecucion (insert);
								if(Request!=null && Request.IsComplete)
								{
									EjecucionMemoria.QuitarMemoriaGlobal(insert.ODTID);
									EjecucionMemoria.QuitarMemoriaEjecucion(insert.ODTID);
								

								}else
								{
									NoExec.Add(ejecucion [CountExec]);
								}
							}else
							{
								NoExec.Add(ejecucion [CountExec]);
								
							}
						} catch (Exception exc)
						{
							NoExec.Add(ejecucion [CountExec]);
						}
						finally {
							if (WS != null)
								WS.Close ();
						}
						CountExec++;
					}
					Utils.MemoriaEjecucion = NoExec;
				}

				if (cancelacion!=null)
				{
					List<DTOODTCancelacion> NoCanc = new List<DTOODTCancelacion> ();
					int CountCanc = 0;
					foreach (DTOODTCancelacion insert in cancelacion) 
					{
						try {
							int detalle = await Utils.CargarFoto (insert.ODTID.ToString (), insert.Foto.Foto,ApplicationContext,insert.Foto.FechaCreacion.Day.ToString()+"/"+insert.Foto.FechaCreacion.Month.ToString()+"/"+insert.Foto.FechaCreacion.Year.ToString());
							if(detalle!=0)
							{
								insert.FotoID = detalle;
								insert.Foto.Foto=null;
								WS=Utils.InitializeServiceClient();
								OperationStatus Request =  WS.CancelODT (insert);
								if(Request!=null && Request.IsComplete)
								{
									EjecucionMemoria.QuitarMemoriaGlobal(insert.ODTID);
									EjecucionMemoria.QuitarMemoriaCancelacion(insert.ODTID);

								}else
								{
									NoCanc.Add(cancelacion [CountCanc]);
								}
							}else
							{
								NoCanc.Add(cancelacion [CountCanc]);
							}
						} catch (Exception exc) 
						{
							NoCanc.Add (cancelacion [CountCanc]);

						}
						finally {
							if (WS != null)
								WS.Close ();
						}
						CountCanc++;
					}
					Utils.MemoriaCancelacion = NoCanc;

				}

				if (pospone != null) 
				{
					List<DTOODTPospuesta> NoPosp = new List<DTOODTPospuesta> ();
					int CountPosp = 0;
					foreach (DTOODTPospuesta insert in pospone) 
					{
						try {
							int detalle = await Utils.CargarFoto (insert.ODTID.ToString (), insert.Foto.Foto,ApplicationContext,insert.Foto.FechaCreacion.Day.ToString()+"/"+insert.Foto.FechaCreacion.Month.ToString()+"/"+insert.Foto.FechaCreacion.Year.ToString());

							if(detalle!=0)
							{
								insert.FotoID = detalle;
								insert.Foto.Foto=null;
								WS=Utils.InitializeServiceClient();
								OperationStatus Request =  WS.PostponeODT (insert);
								if(Request!=null && Request.IsComplete)
								{
									EjecucionMemoria.QuitarMemoriaGlobal(insert.ODTID);
									EjecucionMemoria.QuitarMemoriaPosponer(insert.ODTID);

								}else
								{
									NoPosp.Add(pospone [CountPosp]);
								}
							}else
							{
								NoPosp.Add(pospone [CountPosp]);
							}
						} catch (Exception exc)
						{
							NoPosp.Add (pospone[CountPosp]);
						}
						finally {
							if (WS != null)
								WS.Close ();
						}
						CountPosp++;
					}
					Utils.MemoriaPosponer = NoPosp;
				}

			}catch(Exception ex) {
				throw ex;
			}
		}

		public static Bitmap ChangeSizeBitmap(Bitmap pImagen, int pAncho, int pAlto)
		{
			int width = pImagen.Width;
			int height = pImagen.Height;
			int newWidth = pAncho;
			int newHeight = pAlto;
			float scaleWidth = ((float) newWidth) / width;
			float scaleHeight = ((float) newHeight) / height;
			Matrix matrix = new Matrix();
			matrix.PostScale(scaleWidth, scaleHeight);
			Bitmap resizedBitmap = Bitmap.CreateBitmap(pImagen, 0, 0,width, height, matrix, true);
			return resizedBitmap;


		}


	








	}
}


