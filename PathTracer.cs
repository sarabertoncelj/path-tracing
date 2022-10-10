using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PathTracer.Samplers;

namespace PathTracer
{
  class PathTracer
  {
    public Spectrum Li(Ray r, Scene s)
    {
      var L = Spectrum.ZeroSpectrum;
      var beta = Spectrum.Create(1);
      var n_bounces = 0;
      while (n_bounces < 20)
      {
        var (dist, isect) = s.Intersect(r);
        if (isect == null)
          break;
        Vector3 wo = -r.d;
        if (isect.Obj is Light)
        {
          if (n_bounces == 0) 
          {
            L.AddTo(beta * isect.Le(wo));
          }
          break;
        }
        L.AddTo(beta * Light.UniformSampleOneLight(isect, s));
        if (isect.Obj is Shape obj)
        {
          (Spectrum f, Vector3 wi, double pr, bool specular) = obj.BSDF.Sample_f(wo, isect);
          beta = beta * f * Math.Abs(Vector3.Dot(wi, isect.Normal)) / pr;
          r = isect.SpawnRay(wi);
        }
        if (n_bounces > 3)
        {
          var q = 1 - beta.Max();
          if (ThreadSafeRandom.NextDouble() < q)
          {
            break;
          }
          beta = beta / (1 - q);
        }
        n_bounces++;
      }
      /* Implement */
      return L;
    }

  }
}
