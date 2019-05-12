using NUnit.Framework;

namespace SolarEdge.Monitor.Tests.Inverter.Model
{
    [TestFixture]
    public class ReaderShould
    {
        [Test]
        public void CorrectlyReadInverter()
        {
            var data = new byte[] { 0, 101, 0, 50, 0, 119, 0, 119, 255, 255, 255, 255, 255, 254, 9, 104, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 95, 185, 255, 254, 195, 147, 255, 253, 112, 42, 255, 254, 58, 119, 255, 254, 33, 86, 255, 254, 0, 1, 22, 177, 0, 0, 25, 92, 255, 252, 14, 248, 255, 255, 97, 46, 255, 254, 128, 0, 12, 115, 128, 0, 128, 0, 255, 254, 0, 4 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Inverter>(data);

            Assert.That(actual.Phase, Is.EqualTo(SolarEdge.Monitor.Inverter.Model.Phase.SinglePhase));
            Assert.That(actual.OperatingState, Is.EqualTo(SolarEdge.Monitor.Inverter.Model.OperatingState.Producing));

            Assert.That(actual.AcFrequency, Is.EqualTo(50.067).Within(0.1));
            Assert.That(actual.AcLifetimeProduction, Is.EqualTo(71345));
            Assert.That(actual.AcPowerValue, Is.EqualTo(245.05).Within(0.1));
            Assert.That(actual.ApparentPower, Is.EqualTo(287.14).Within(0.1));
            Assert.That(actual.DcCurrentValue, Is.EqualTo(0.6492).Within(0.1));
            Assert.That(actual.DcPowerValue, Is.EqualTo(248.78).Within(0.1));
            Assert.That(actual.DcVoltageValue, Is.EqualTo(383.2).Within(0.1));
            Assert.That(actual.HeatSinkTemperature, Is.EqualTo(31.87).Within(0.1));
            Assert.That(actual.PowerFactor, Is.EqualTo(85.34).Within(0.1));
            Assert.That(actual.ReactivePower, Is.EqualTo(149.67).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterInfo()
        {
            var data = new byte[] { 0, 1, 0, 65, 87, 97, 116, 116, 78, 111, 100, 101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 87, 78, 68, 45, 51, 89, 45, 52, 48, 48, 45, 77, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 69, 120, 112, 111, 114, 116, 43, 73, 109, 112, 111, 114, 116, 0, 0, 0, 51, 49, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 53, 48, 49, 57, 52, 53, 52, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 203, 0, 105 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1Info>(data);

            Assert.That(actual.DeviceId, Is.EqualTo(1));
            Assert.That(actual.Length, Is.EqualTo(65));
            Assert.That(actual.Manufacturer, Is.EqualTo("WattNode"));
            Assert.That(actual.Model, Is.EqualTo("WND-3Y-400-MB"));
            Assert.That(actual.Option, Is.EqualTo("Export+I"));
            Assert.That(actual.Version, Is.EqualTo("31"));
            Assert.That(actual.SerialNumber, Is.EqualTo("5019454"));
            Assert.That(actual.DeviceAddress, Is.EqualTo(1));
            Assert.That(actual.ModbusType, Is.EqualTo(SolarEdge.Monitor.Inverter.Model.ModbusType.WyeConnectThreePhase));
        }

        [Test]
        public void CorrectlyReadMeterCurrent()
        {
            // 0, 105, 
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 255, 255 } ;

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1Current>(data);

            Assert.That(actual.Total, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseA, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseB, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseC, Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterVoltages()
        {
            var data = new byte[] { 95, 121, 95, 121, 0, 0, 0, 0, 94, 156, 94, 143, 0, 0, 94, 169, 255, 254, 19, 148, 255, 254 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1Voltage>(data);

            Assert.That(actual.AveragePhaseToNeutral, Is.EqualTo(244.41).Within(0.1));
            Assert.That(actual.PhaseAToNeutral, Is.EqualTo(244.41).Within(0.1));
            Assert.That(actual.PhaseBToNeutral, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseCToNeutral, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.AveragePhaseToLine, Is.EqualTo(242.19).Within(0.1));
            Assert.That(actual.PhaseAToPhaseB, Is.EqualTo(242.02).Within(0.1));
            Assert.That(actual.PhaseBToPhaseC, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseCToPhaseA, Is.EqualTo(242.33).Within(0.1));
            Assert.That(actual.ACFrequency, Is.EqualTo(50.12).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterRealPower()
        {
            var data = new byte[] { 0, 69, 0, 69, 0, 0, 0, 0, 0, 0 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1RealPower>(data);

            Assert.That(actual.Total, Is.EqualTo(69.0).Within(0.1));
            Assert.That(actual.PhaseA, Is.EqualTo(69.0).Within(0.1));
            Assert.That(actual.PhaseB, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseC, Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterApparentPower()
        {
            var data = new byte[] { 1, 171, 1, 171, 0, 0, 0, 0, 0, 0 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1ApparentPower>(data);

            Assert.That(actual.Total, Is.EqualTo(427.0).Within(0.1));
            Assert.That(actual.PhaseA, Is.EqualTo(427.0).Within(0.1));
            Assert.That(actual.PhaseB, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseC, Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterReactivePower()
        {
            var data = new byte[] { 254, 91, 254, 91, 0, 0, 0, 0, 0, 0 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1ReactivePower>(data);

            Assert.That(actual.Total, Is.EqualTo(-421.0).Within(0.1));
            Assert.That(actual.PhaseA, Is.EqualTo(-421.0).Within(0.1));
            Assert.That(actual.PhaseB, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseC, Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterPowerFactor()
        {
            var data = new byte[] { 252, 104, 252, 104, 0, 0, 0, 0, 255, 254 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1PowerFactor>(data);

            Assert.That(actual.Average, Is.EqualTo(-9.2).Within(0.1));
            Assert.That(actual.PhaseA, Is.EqualTo(-9.2).Within(0.1));
            Assert.That(actual.PhaseB, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseC, Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void CorrectlyReadMeterAccumulatedRealEnergy()
        {
            var data = new byte[] {
                0, 0, 142, 89, 0, 0, 142, 89, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 3, 48, 127,
                0, 3, 48, 127, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0 }; //, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var actual = SolarEdge.Monitor.Inverter.Model.Reader.Default.Read<SolarEdge.Monitor.Inverter.Model.Meter1AccumulatedRealEnergy>(data);

            Assert.That(actual.TotalExported, Is.EqualTo(0.1).Within(0.1));
            Assert.That(actual.PhaseAExported, Is.EqualTo(0.1).Within(0.1));
            Assert.That(actual.PhaseBExported, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseCExported, Is.EqualTo(0).Within(0.1));

            Assert.That(actual.TotalImported, Is.EqualTo(3.0).Within(0.1));
            Assert.That(actual.PhaseAImported, Is.EqualTo(3.0).Within(0.1));
            Assert.That(actual.PhaseBImported, Is.EqualTo(0).Within(0.1));
            Assert.That(actual.PhaseCImported, Is.EqualTo(0).Within(0.1));
        }
    }
}
