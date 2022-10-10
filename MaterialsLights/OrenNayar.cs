using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer
{
   public class OrenNayar : BxDF
  {
    private Spectrum kd;
    private double roughness;
    public OrenNayar(Spectrum kd, double roughness)
    {
      this.kd = kd;
      this.roughness = roughness;
    }

    public override Spectrum f(Vector3 wo, Vector3 wi)
    {
      if (!Utils.SameHemisphere(wo, wi))
        return Spectrum.ZeroSpectrum;
      var ro = Math.Sqrt(Math.Pow(wo.x, 2) + Math.Pow(wo.y, 2) + Math.Pow(wo.z, 2));
      var ri = Math.Sqrt(Math.Pow(wi.x, 2) + Math.Pow(wi.y, 2) + Math.Pow(wi.z, 2));
      var to = Math.Acos(Utils.CosTheta(wo));
      var ti = Math.Acos(Utils.CosTheta(wi));
      var po = Math.Asin(Utils.SinPhi(wo));
      var pi = Math.Asin(Utils.SinPhi(wi));
      var a = 1 - (Math.Pow(roughness, 2) / (2 * (Math.Pow(roughness, 2) + 0.33)));
      var b = 0.45 * (Math.Pow(roughness, 2) / (Math.Pow(roughness, 2) + 0.09));
      var alpha = Math.Max(ti, to);
      var beta = Math.Min(ti, to);
      return (kd * Utils.PiInv) * (a + b * Math.Max(0, Math.Cos(pi - po)) * Math.Sin(alpha) * Math.Tan(beta));
    }

    public override (Spectrum, Vector3, double) Sample_f(Vector3 wo)
    {
      var wi = Samplers.CosineSampleHemisphere();
      if (wo.z < 0)
        wi.z *= -1;
      double pdf = Pdf(wo, wi);
      return (f(wo, wi), wi, pdf);
    }

    public override double Pdf(Vector3 wo, Vector3 wi)
    {
      if (!Utils.SameHemisphere(wo, wi))
        return 0;

      return Math.Abs(wi.z) * Utils.PiInv; // wi.z == cosTheta
    }
  }
}
