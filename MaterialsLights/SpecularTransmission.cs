using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer
{
   public class SpecularTransmission : BxDF
  {
    private Spectrum r;
    private FresnelDielectric fresnel;

    public override bool IsSpecular => true;

    public SpecularTransmission(Spectrum r, double fresnel1, double fresnel2)
    {
      this.r = r;
      fresnel = new FresnelDielectric(fresnel1, fresnel2);
    }

    /// <summary>
    /// f of perfect specular transmission is zero (probability also)
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="wi"></param>
    /// <returns></returns>
    public override Spectrum f(Vector3 wo, Vector3 wi)
    {
      return Spectrum.ZeroSpectrum;
    }

    /// <summary>
    /// Sample returns a single possible direction
    /// </summary>
    /// <param name="woL"></param>
    /// <returns></returns>
    public override (Spectrum, Vector3, double) Sample_f(Vector3 wo)
    {
      // perfect specular reflection

      Vector3 wi = new Vector3(-wo.x, -wo.y, wo.z);
      Spectrum fr = fresnel.Evaluate(Utils.CosTheta(wi));
      Spectrum ft = (1 - fr.Max()) * r;
      Vector3 norm = new Vector3(0, 0, 1);
      var eta = fresnel.EtaI / fresnel.EtaT;
      var cos_ti = -Vector3.Dot(norm, wi);
      var sin_tt = Math.Pow(eta, 2) * (1 - Math.Pow(cos_ti, 2));
      var cos_tt = Math.Sqrt(1 - sin_tt);
      Vector3 wt = eta * -wi + (eta * cos_ti - cos_tt) * norm;
      return (ft / Utils.AbsCosTheta(wt), wt, 1);
    }

    /// <summary>
    /// Probability is 0
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="wi"></param>
    /// <returns></returns>
    public override double Pdf(Vector3 wo, Vector3 wi)
    {
      return 0;
    }
  }
}
