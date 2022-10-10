using System;
using System.Drawing;
using MathNet.Numerics.Integration;

namespace PathTracer
{
  class Sphere : Shape
  {
    public double Radius { get; set; }
    public Sphere(double radius, Transform objectToWorld)
    {
      Radius = radius;
      ObjectToWorld = objectToWorld;
    }

    public override (double?, SurfaceInteraction) Intersect(Ray ray)
    {
      Ray r = WorldToObject.Apply(ray);

      // TODO: Compute quadratic sphere coefficients

      // TODO: Initialize _double_ ray coordinate values
      var a = Math.Pow(r.d.x, 2) + Math.Pow(r.d.y, 2) + Math.Pow(r.d.z, 2);
      var b = 2 * (r.d.x * r.o.x + r.d.y * r.o.y + r.d.z * r.o.z);
      var c = Math.Pow(r.o.x, 2) + Math.Pow(r.o.y, 2) + Math.Pow(r.o.z, 2) - Math.Pow(Radius, 2);
      // TODO: Solve quadratic equation for _t_ values
      (bool solvable, double t0, double t1) = Utils.Quadratic(a, b, c);
      if (!solvable || t1 <= 0)
      {
        return (null, null);
      }
      // TODO: Check quadric shape _t0_ and _t1_ for nearest intersection
      double s_hit = t0;
      if (s_hit <= Renderer.Epsilon)
      {
        s_hit = t1;
      }
      // TODO: Compute sphere hit position and $\phi$
      var p_hit = r.Point(s_hit);
      var phi = Math.Atan2(p_hit.y, p_hit.x);
      if (phi < 0)
      {
        phi = phi + 2 * Math.PI;
      }
      var dpdu = new Vector3 (-phi * p_hit.y, phi * p_hit.x, 0);
      var interaction = new SurfaceInteraction(p_hit, p_hit.Clone().Normalize(), -r.d, dpdu, this);
      // TODO: Return shape hit and surface interaction
      return (s_hit, ObjectToWorld.Apply(interaction));

      // A dummy return example
      double dummyHit = 0.0;
      Vector3 dummyVector = new Vector3(0,0,0);
      SurfaceInteraction dummySurfaceInteraction = new SurfaceInteraction(dummyVector, dummyVector, dummyVector, dummyVector, this);
      return (dummyHit, dummySurfaceInteraction);
    }

    public override (SurfaceInteraction, double) Sample()
    {
      // TODO: Implement Sphere sampling
      var c = new Vector3(0, 0, 0);
      var p = c + Radius * Samplers.CosineSampleHemisphere();

      var norm = ObjectToWorld.ApplyNormal(p);
      var phi = Math.Atan2(p.y, p.x);
      if (phi < 0)
      {
        phi = phi + 2 * Math.PI;
      }
      var wo = p;
      var dpdu = new Vector3(-phi * p.y, phi * p.x, 0);
      var interaction = new SurfaceInteraction(p, norm, wo, dpdu, this);
      return (ObjectToWorld.Apply(interaction), Pdf(interaction, p));
      // TODO: Return surface interaction and pdf

      // A dummy return example
      double dummyPdf = 1.0;
      Vector3 dummyVector = new Vector3(0,0,0);
      SurfaceInteraction dummySurfaceInteraction = new SurfaceInteraction(dummyVector, dummyVector, dummyVector, dummyVector, this);
      return (dummySurfaceInteraction, dummyPdf);
    }

    public override double Area() { return 4 * Math.PI * Radius * Radius; }

    public override double Pdf(SurfaceInteraction si, Vector3 wi)
    {
      return 1 / Area();
    }

  }
}
