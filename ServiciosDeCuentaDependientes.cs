using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banco.Entidades;
using Banco.Servicios.ServiciosDeTerceros;
//Operaciones de cuentas como validacion de usuario y contraseñas y cambios de divisa
namespace Banco.Servicios
{
    public class ServiciosDeCuentaDependientes
    {
        IRepositorioUsuarios _repositorioUsuarios;
        IRepositorioConfiguraciones _repositorioConfiguraciones;
        IServicioExternoBuro _servicioExternoBuro;
        IServicioExternoSPEI _servicioExternoSPEI;
        IServicioExternoTipoDeCambio _servicioExternoTipoDeCambio;

        public ServiciosDeCuentaDependientes()
        {
            
        }

        public ServiciosDeCuentaDependientes(IRepositorioUsuarios repositorioUsuarios,
                                                IRepositorioConfiguraciones repositorioConfiguraciones,
                                                IServicioExternoBuro servicioExternoBuro,
                                                IServicioExternoSPEI servicioExternoSPEI,
                                                IServicioExternoTipoDeCambio servicioExternoTipoDeCambio)
        {
            _repositorioUsuarios = repositorioUsuarios;
            _repositorioConfiguraciones = repositorioConfiguraciones;
            _servicioExternoBuro = servicioExternoBuro;
            _servicioExternoSPEI = servicioExternoSPEI;
            _servicioExternoTipoDeCambio = servicioExternoTipoDeCambio;
        }


        public bool Login(string nombreUsuario, string password)
        {
            var result = false;
           // var servicioUsuario = new RepositorioUsuarios();
            var usuario = _repositorioUsuarios.SeleccionarUsuario(nombreUsuario);//servicioUsuario

            if (usuario == null)
            {
                result = false;
            }
            else
            {
                if (usuario.EstaActivo)
                {
                    if (usuario.Password == password)
                    {
                        result = true;
                    }
                }
            }
           
            return result;
        }
        
        public bool SolicitarTarjeta(Usuario usuario, TarjetaDeCredito tarjetaSolicitada)
        {
            //verificar si el usuario ya tiene una tarjeta de ese tipo
            if (usuario.Cuentas.Any(c => c.GetType() == tarjetaSolicitada.GetType()))
            {
                return false;
            }

            //consultar # maximo de tarjetas
            //var servicioConfiguracion = new RepositorioConfiguraciones();
            int numMaximoDeTarjetas = _repositorioConfiguraciones.SeleccionarMaximoDeTarjetasPorUsuario();
            if (usuario.Cuentas.Where(c => c is TarjetaDeCredito).Count() > numMaximoDeTarjetas)
            {
                return false;
            }

            //consultar buro de credito
            //var servicioBuro = new ServicioExternoBuro();
            var calificacionBuro = _servicioExternoBuro.ConsultarBuro(usuario.RFC);
            if (tarjetaSolicitada is TarjetaClasica && calificacionBuro < 30)
            {
                return false;
            }
            else if (tarjetaSolicitada is TarjetaOro && calificacionBuro < 65)
            {
                return false;
            }
            else if (tarjetaSolicitada is TarjetaPlatino && calificacionBuro < 85)
            {
                return false;
            }

            return true;
        }

        public void TransferirATerceros(ICuentaBancaria origen, ICuentaBancaria destino, Moneda cantidad)
        {
            if (destino is CuentaDeAhorroExterna == false)
            {
                throw new InvalidOperationException("Cuenta destino no es de otro banco");
            }
            if (origen.Balance.Divisa != destino.Balance.Divisa)
            {
                throw new InvalidOperationException("Divisas diferentes");
            }
            if (origen.Titular.Id != destino.Titular.Id)
            {
                throw new InvalidOperationException("Usuarios diferentes");
            }

            var cuentaExterna = destino as CuentaDeAhorroExterna;
            //var serviciosSpei = new ServicioExternoSPEI();
            var exito = _servicioExternoSPEI.EnviarSpei(cuentaExterna.Banco, cuentaExterna.CLABE, cantidad.Cantidad);
            if (exito)
            {
                origen.Balance = (Moneda)origen.Balance.Restar(cantidad);

                //cobrar comision de 1%
                FuncionesComunes.RestarCantidad(origen, new Moneda(cantidad.Cantidad * (decimal)0.01, cantidad.Divisa));
            }
            else
            {
                throw new ApplicationException("Error al enviar SPEI.");
            }
        }

        public Moneda ConvertirMoneda(Moneda origen, Divisa divisaDeseada)
        {
            Moneda result = null;

            //var servicioExchange = new ServicioExternoTipoDeCambio();
            var tipoDeCambio = _servicioExternoTipoDeCambio.TipoDeCambio(origen.Divisa, divisaDeseada);
            result = new Moneda(origen.Cantidad * tipoDeCambio, divisaDeseada);

            return result;
        }

        public Moneda ConvertirPesosADolares(decimal pesos)
        {
            Moneda result = null;

            //var servicioExchange = new ServicioExternoTipoDeCambio();
            var tipoDeCambio = _servicioExternoTipoDeCambio.TipoDeCambio(Divisa.MXN, Divisa.USD);
            result = new Moneda(pesos * tipoDeCambio, Divisa.USD);

            return result;
        }

        public Moneda ConvertirPesosAEuros(decimal pesos)
        {
            Moneda result = null;
            result = ConvertirMoneda(new Moneda(pesos, Divisa.MXN), Divisa.EUR);           
            return result;
        }
    }
}
