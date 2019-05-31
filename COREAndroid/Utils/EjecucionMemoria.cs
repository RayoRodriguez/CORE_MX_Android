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
using System.Threading.Tasks;
using SAO.Entity.Operation;
using SAO.Bss.Helper;

namespace POL_CORE
{
	class EjecucionMemoria
	{
		


		public static async Task<int> Finalizar(DTOODTEjecucion insert,Context ApplicationContext)
		{
			int regreso = 0;
			OperationClient WS=null;
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
					if(Request.IsComplete)
					{
						regreso=1;
					}
				}
			} catch (Exception exc)
			{
				regreso = 0;

			}
			finally {

				if (WS != null)
					WS.Close ();
			}
			return regreso;
		}


		public static async Task<int>  Cancelar (DTOODTCancelacion insert,Context ApplicationContext)
		{
			int regreso = 0;
			OperationClient WS=null;
			try 
			{
				int detalle = await Utils.CargarFoto (insert.ODTID.ToString (), insert.Foto.Foto,ApplicationContext,insert.Foto.FechaCreacion.Day.ToString()+"/"+insert.Foto.FechaCreacion.Month.ToString()+"/"+insert.Foto.FechaCreacion.Year.ToString());
				if(detalle!=0)
				{
					insert.FotoID = detalle;
					insert.Foto.Foto=null;
					WS=Utils.InitializeServiceClient();
					OperationStatus Request =  WS.CancelODT (insert);
					if(Request.IsComplete)
					{
						regreso=1;
					}
				}
			} catch (Exception exc) 
			{
				regreso = 0;
			}
			finally {
				if (WS != null)
					WS.Close ();
			}
			return regreso;
		}



		public static async Task<int> Posponer(DTOODTPospuesta insert,Context ApplicationContext)
		{
			OperationClient WS=null;
			int regreso = 0;
			try
			 {
				int detalle = await Utils.CargarFoto (insert.ODTID.ToString (), insert.Foto.Foto,ApplicationContext,insert.Foto.FechaCreacion.Day.ToString()+"/"+insert.Foto.FechaCreacion.Month.ToString()+"/"+insert.Foto.FechaCreacion.Year.ToString());
				if(detalle!=0)
				{
					insert.FotoID = detalle;
					insert.Foto.Foto=null;
					WS=Utils.InitializeServiceClient();
					OperationStatus Request = WS.PostponeODT (insert);
					if(Request.IsComplete)
					{
						regreso=1;
					}
				}
			} catch (Exception exc)
			{
				regreso = 0;
			}
			finally {
				if (WS != null)
					WS.Close ();
			}
			return regreso;
		}

		public static DTOODTEjecucion FindEjecucion (DTOODTCustom odt)
		{
			DTOODTEjecucion RetExec = null;
			List<DTOODTEjecucion> ejecucion = Utils.MemoriaEjecucion;
			if (ejecucion != null) 
			{
				foreach (DTOODTEjecucion ex in ejecucion) 
				{
					if (ex.ODTID == odt.ID) 
					{
						RetExec = ex;
					}

				}
			}

			return RetExec;



		} 

		public static DTOODTCancelacion FindCancelacion (DTOODTCustom odt)
		{
			DTOODTCancelacion RetExec = null;
			List<DTOODTCancelacion> cancelacion = Utils.MemoriaCancelacion;
			if (cancelacion != null) 
			{
				foreach (DTOODTCancelacion ex in cancelacion) 
				{
					if (ex.ODTID == odt.ID) 
					{
						RetExec = ex;
					}

				}
			}

			return RetExec;

		} 

		public static DTOODTPospuesta FindPospuesta (DTOODTCustom odt)
		{
			DTOODTPospuesta RetExec = null;
			List<DTOODTPospuesta> pospone = Utils.MemoriaPosponer;
			if (pospone != null) 
			{
				foreach (DTOODTPospuesta ex in pospone) 
				{
					if (ex.ODTID == odt.ID) 
					{
						RetExec = ex;
					}

				}
			}

			return RetExec;

		} 


		public static void QuitarMemoriaGlobal(int idodt)
		{
			DTOODTCustom[] Memoria = Utils.Memoria;
			List<DTOODTCustom> mem=Memoria.ToList<DTOODTCustom>();
			if (Memoria != null) {
				for (int i = 0; i < mem.Count; i++) 
				{
					if(mem[i].ID==idodt)
					{
						mem.RemoveAt(i);
						break;

					}
				}
				Utils.Memoria = mem.ToArray ();
			}

		}

		public static void QuitarMemoriaEjecucion(int idodt)
		{
			List<DTOODTEjecucion> Mem = Utils.MemoriaEjecucion;
			if (Mem != null) 
			{
				for (int i = 0; i < Mem.Count; i++) 
				{
					if(Mem[i].ODTID==idodt)
					{
						Mem.RemoveAt (i);
						break;
					}
				}
				Utils.MemoriaEjecucion = Mem;
			}


		}

		public static void QuitarMemoriaCancelacion(int idodt)
		{
			List<DTOODTCancelacion> Mem = Utils.MemoriaCancelacion;
			if (Mem != null) 
			{
				for (int i = 0; i < Mem.Count; i++) 
				{
					if(Mem[i].ODTID==idodt)
					{
						Mem.RemoveAt (i);
						break;
					}
				}
				Utils.MemoriaCancelacion = Mem;
			}


		}

		public static void QuitarMemoriaPosponer(int idodt)
		{
			List<DTOODTPospuesta> Mem = Utils.MemoriaPosponer;
			if (Mem != null) 
			{
				for (int i = 0; i < Mem.Count; i++) 
				{
					if(Mem[i].ODTID==idodt)
					{
						Mem.RemoveAt (i);
						break;
					}
				}
				Utils.MemoriaPosponer = Mem;
			}


		}




	}
}

