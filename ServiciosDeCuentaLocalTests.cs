using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banco.Servicios;
using Banco.Entidades;

namespace Banco.UnitTests
{
    [TestFixture]
    public class ServiciosDeCuentaLocalTests
    {                
        #region Cambiar NIP
        [Test]
        public void CambiarNIP_ACuentaDeAhorroExterna_LanzaExcepcion()
        {
            //Arrange
            var servicio = new ServiciosDeCuentaLocal();
            var cuenta = new CuentaDeAhorroExterna();
            var nip1 = 1234;
            var nip2 = 1234;

            //Act
            void metodoAprobar() =>  servicio.CambiarNIP(cuenta, nip1, nip2); 
            

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }

        [Test]
        public void CambiarNIP_NIP1MenorACuatroDigitos_LanzaExcepcion()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 123;// menor a 4 digitos
            var nip2 = 1234;
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            void metodoAprobar() => servicio.CambiarNIP(cuenta, nip1,nip2);

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }

        [Test]
        public void CambiarNIP_NIP1MayorACuatroDigitos_LanzaExcepcion()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 123455;//mayor a 4 digitos
            var nip2 = 1234;
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            void metodoAprobar() => servicio.CambiarNIP(cuenta, nip1, nip2);

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }

        [Test]
        public void CambiarNIP_NIP2MenorACuatroDigitos_LanzaExcepcion()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 1234;
            var nip2 = 123;//Menor a 4 digitos
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            void metodoAprobar() => servicio.CambiarNIP(cuenta, nip1, nip2);

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }

        [Test]
        public void CambiarNIP_NIP2MayorACuatroDigitos_LanzaExcepcion()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 1234;
            var nip2 = 123455;//mayor a 4 digitos
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            void metodoAprobar() => servicio.CambiarNIP(cuenta, nip1, nip2);

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }

        [Test]
        public void CambiarNIP_NIPsDiferentes_LanzaExcepcion()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 1234;//Verificar que sean diferentes
            var nip2 = 1235;
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            void metodoAprobar() => servicio.CambiarNIP(cuenta, nip1, nip2);

            //Assert
            Assert.Throws<InvalidOperationException>(metodoAprobar);
        }
        
        [Test]
        public void CambiarNIP_NIPsvalidos_ActualizaCuentaConNuevoNIP()
        {
            //Arrange
            var cuenta = new CuentaDeAhorro();
            var nip1 = 1234;
            var nip2 = 1234;
            var servicio = new ServiciosDeCuentaLocal();

            //Act
            servicio.CambiarNIP(cuenta, nip1, nip2);

            //Assert
            Assert.AreEqual(nip1, cuenta.NIP);
        }

        #endregion Cambiar NIP

        #region RetirarDeCajero
        [Test]
        public void RetirarDeCajero_OnlyDivisaMXN ()
        {
            //Arrange
            var servicio = new ServiciosDeCuentaLocal();
            var cuenta = new CuentaDeAhorro() {Balance = new Moneda(1000, Divisa.EUR), NIP = 1234 };
            int nip = 1234;

            //Act
            void MetodoAprobar() => servicio.RetirarDeCajero(cuenta,nip,(decimal)100);

            //Arrange
            Assert.Throws<InvalidOperationException>(MetodoAprobar);
        }

        [Test]
        public void RetirarDeCajero_ValidaNips()
        {
            //Arrange
            var servicio = new ServiciosDeCuentaLocal();
            var cuenta = new CuentaDeAhorro() { Balance = new Moneda(1000, Divisa.MXN), NIP = 1234 };
            int nip = 1235;

            //Act
            void MetodoAprobar() => servicio.RetirarDeCajero(cuenta, nip, (decimal)100);

            //Arrange
            Assert.Throws<InvalidOperationException>(MetodoAprobar);
        }

        [Test]
        public void RetirarDeCajero_AgregarComision_CuentaExterna()
        {
            //Arrange
            var servicio = new ServiciosDeCuentaLocal();
            var cuenta = new CuentaDeAhorroExterna() { Balance = new Moneda(1000, Divisa.MXN), NIP = 1234 };
            int nip = 1234;

            //Act
            servicio.RetirarDeCajero(cuenta, nip, 100m);

            //Arrange
            Assert.AreEqual(new Moneda(870, Divisa.MXN), cuenta.Balance);
        }

        [Test]
        public void RetirarDeCajero_AgregarComision_CreditCard()
        {
            //Arrange
            var servicio = new ServiciosDeCuentaLocal();
            var cuenta = new TarjetaClasica() { Balance = new Moneda(1000, Divisa.MXN), NIP = 1234 };
            int nip = 1234;

            //Act
            servicio.RetirarDeCajero(cuenta, nip, 100m);

            //Arrange
            Assert.AreEqual(new Moneda(894, Divisa.MXN), cuenta.Balance);
        }


        #endregion RetirarDeCajero

        #region TransferirEntreCuentasPropias
        [Test] 
        public void TransferirEntreCuentasPropias_DivisasDiferentes()
        {
            //Arrange
            var Coin = new Moneda(100, Divisa.EUR);
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 456;
            var CuentaOri = new CuentaDeAhorro()
            { Balance = new Moneda (1000, Divisa.EUR),
                Titular = User1};
            var CuentaFin = new CuentaDeAhorro()
            {Balance = new Moneda(1000, Divisa.USD),
                Titular = User2};
            var Servicio = new ServiciosDeCuentaLocal();
            //Act
            void MetodoAverificar() => Servicio.TransferirEntreCuentasPropias(CuentaOri,CuentaFin, Coin);
            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAverificar);
        }

        [Test]
        public void TransferirEntreCuentasPropias_UsuariosDiferentes()
        {
            //Arrange
            var Coin = new Moneda(100, Divisa.EUR);
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 459;
            var CuentaOri = new CuentaDeAhorro()
            {
                Balance = new Moneda(1000, Divisa.EUR),
                Titular = User1
            };
            var CuentaFin = new CuentaDeAhorro()
            {
                Balance = new Moneda(1000, Divisa.EUR),
                Titular = User2
            };
            var Servicio = new ServiciosDeCuentaLocal();
            //Act
            void MetodoAverificar() => Servicio.TransferirEntreCuentasPropias(CuentaOri, CuentaFin, Coin);
            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAverificar);

        }

        [Test]
        public void TransferirEntreCuentasPropias_Cantidades_Correctas()
        {
            //Arrange
            var Coin = new Moneda(100, Divisa.EUR);
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 456;
            var CuentaOri = new CuentaDeAhorro()
            {
                Balance = new Moneda(1000, Divisa.EUR),
                Titular = User1
            };
            var CuentaFin = new CuentaDeAhorro()
            {
                Balance = new Moneda(1000, Divisa.EUR),
                Titular = User2
            };
            var Servicio = new ServiciosDeCuentaLocal();
            //Act
            Servicio.TransferirEntreCuentasPropias(CuentaOri, CuentaFin, Coin);
            //Assert
            Assert.AreEqual(new Moneda (900, Divisa.EUR), CuentaOri.Balance);
            Assert.AreEqual(new Moneda(1100, Divisa.EUR), CuentaFin.Balance);

        }

        #endregion TransferirEntreCuentasPropias

        #region PagarTarjeta
        [Test]
        public void PagarTarjeta_Divisas_Diferentes ()
        {
            //Arange
            var Cuenta = new CuentaDeAhorro() {Balance = new Moneda(1000, Divisa.MXN) };
            var CreditC = new TarjetaClasica() { Balance = new Moneda(1000, Divisa.EUR) };
            var servicios = new ServiciosDeCuentaLocal();
            var coin = new Moneda(100, Divisa.MXN);
            //ACT
            void MetodoAValidar () => servicios.PagarTarjeta(Cuenta, CreditC, coin);

            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAValidar);
        }

        [Test]
        public void PagarTarjeta_Titular_Diferente()
        {
            //Arange
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 459;
            var Cuenta = new CuentaDeAhorro() {Titular = User1,
                Balance = new Moneda(1000, Divisa.MXN) };
            var CreditC = new TarjetaClasica() {Titular = User2,
                Balance = new Moneda(1000, Divisa.MXN) };
            var servicios = new ServiciosDeCuentaLocal();
            var coin = new Moneda(100, Divisa.MXN);
            
            //ACT
            void MetodoAValidar() => servicios.PagarTarjeta(Cuenta, CreditC, coin);

            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAValidar);
        }

        [Test]
        public void PagarTarjeta_Comision_Pago_Otra_TDC()
        {
            //Arange
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 456;
                //Una tarjeta de credito para pagar la TDC actual
            var Cuenta = new TarjetaOro()
            {
                Titular = User1,
                Balance = new Moneda(1000, Divisa.MXN)
            };
            var CreditC = new TarjetaClasica()
            {
                Titular = User2,
                Balance = new Moneda(-1000, Divisa.MXN),
                FechaDeCorte = DateTime.Now.AddDays(2)
            };
            var servicios = new ServiciosDeCuentaLocal();
            var coin = new Moneda(100, Divisa.MXN);

            //ACT
            servicios.PagarTarjeta(Cuenta, CreditC, coin);

            //Assert
            Assert.AreEqual(new Moneda (895m, Divisa.MXN ), Cuenta.Balance);
            Assert.AreEqual(new Moneda(-900m, Divisa.MXN), CreditC.Balance);
        }

        [Test]
        public void PagarTarjeta_Comision_FechaCorte_Vencida()
        {
            //Arange
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 456;
            //Una tarjeta de credito para pagar la TDC actual
            var Cuenta = new CuentaDeAhorro()
            {
                Titular = User1,
                Balance = new Moneda(1000, Divisa.MXN)
            };
            var CreditC = new TarjetaClasica()
            {
                Titular = User2,
                Balance = new Moneda(-1000, Divisa.MXN),
                FechaDeCorte = DateTime.Now.AddDays(-5)//Fecha de corte menor a hoy
            };
            var servicios = new ServiciosDeCuentaLocal();
            var coin = new Moneda(100, Divisa.MXN);

            //ACT
            servicios.PagarTarjeta(Cuenta, CreditC, coin);

            //Assert
            Assert.AreEqual(new Moneda(890m, Divisa.MXN), Cuenta.Balance);
            Assert.AreEqual(new Moneda(-900m, Divisa.MXN), CreditC.Balance);
        }

        [Test]
        public void PagarTarjeta_Comision_FechaCorteVencida_PagoTDC()
        {
            //Arange
            var User1 = new Usuario();
            User1.Id = 456;
            var User2 = new Usuario();
            User2.Id = 456;
            //Una tarjeta de credito para pagar la TDC actual
            var Cuenta = new TarjetaPlatino()
            {
                Titular = User1,
                Balance = new Moneda(1000, Divisa.MXN)
            };
            var CreditC = new TarjetaClasica()
            {
                Titular = User2,
                Balance = new Moneda(-1000, Divisa.MXN),
                FechaDeCorte = DateTime.Now.AddDays(-5)//Fecha de corte menor a hoy
            };
            var servicios = new ServiciosDeCuentaLocal();
            var coin = new Moneda(100, Divisa.MXN);

            //ACT
            servicios.PagarTarjeta(Cuenta, CreditC, coin);

            //Assert
            Assert.AreEqual(new Moneda(885m, Divisa.MXN), Cuenta.Balance);
            Assert.AreEqual(new Moneda(-900m, Divisa.MXN), CreditC.Balance);
        }

        #endregion PagarTarjeta
    }
}
