using UnityEngine;
using Fusion;
using System.Collections;

public class FireTurret : NetworkBehaviour
{

    [Tooltip("Optional audio source to play once when the script starts.")]
    public AudioSource AudioSource;

    //[Tooltip("How long the script takes to fully start. This is used to fade in animations and sounds, etc.")]
    //public float StartTime = 1.0f;
    //
    //[Tooltip("How long the script takes to fully stop. This is used to fade out animations and sounds, etc.")]
    //public float StopTime = 3.0f;
    //
    //[Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
    //public float Duration = 2.0f;

    [Tooltip("Duration of the flame effect.")]
    public float FlameDuration = 2.0f;

    [Tooltip("Duration of the pause between flames.")]
    public float PauseDuration = 2.0f;

    //[Tooltip("How much force to create at the center (explosion), 0 for none.")]
    //public float ForceAmount;
    //
    //[Tooltip("The radius of the force, 0 for none.")]
    //public float ForceRadius;
    //
    //[Tooltip("A hint to users of the script that your object is a projectile and is meant to be shot out from a person or trap, etc.")]
    //public bool IsProjectile;
    //
    //[Tooltip("Particle systems that must be manually started and will not be played on start.")]
    //public ParticleSystem[] ManualParticleSystems;
    //
    //private float startTimeMultiplier;
    //private float startTimeIncrement;
    //
    //private float stopTimeMultiplier;
    //private float stopTimeIncrement;

    public bool isFlameActive;

    ParticleSystem[] particles;
    public float fireRadius = 0.7f;
    public float fireDistance = 13f;
    public int fireDamage = 25;

    //private IEnumerator CleanupEverythingCoRoutine()
    //{
    //    // 2 extra seconds just to make sure animation and graphics have finished ending
    //    yield return new WaitForSeconds(StopTime + 2.0f);
    //
    //    GameObject.Destroy(gameObject);
    //}

    private void StartParticleSystems()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            //if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
            //    System.Array.IndexOf(ManualParticleSystems, p) < 0)
            //{
            if (p.main.startDelay.constant == 0.0f)
            {
                // wait until next frame because the transform may change
                var m = p.main;
                var d = p.main.startDelay;
                d.constant = 0.01f;
                m.startDelay = d;
            }
            RPC_PlayParticle();
            //}
        }
    }

    public override void Spawned()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        if (!HasStateAuthority)
        {
            return;
        }
        Starting = true;

        StartCoroutine(FlameControl());
        // precalculate so we can multiply instead of divide every frame
        //stopTimeMultiplier = 1.0f / StopTime;
        //startTimeMultiplier = 1.0f / StartTime;

        // if this effect has an explosion force, apply that now
        //CreateExplosion(gameObject.transform.position, ForceRadius, ForceAmount);

        // start any particle system that is not in the list of manual start particle systems
        StartParticleSystems();
    }

    private IEnumerator FlameControl()
    {
        while (true)
        {
            // Start the flame effect
            isFlameActive = true;
            StartParticleSystems();

            // Wait for the duration of the flame
            yield return new WaitForSeconds(FlameDuration);

            // Stop the flame effect
            isFlameActive = false;
            StopParticleSystems();

            // Wait for the pause duration
            yield return new WaitForSeconds(PauseDuration);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // reduce the duration
        //Duration -= Time.deltaTime;
        //if (Stopping)
        //{
        //    // increase the stop time
        //    stopTimeIncrement += Time.deltaTime;
        //    if (stopTimeIncrement < StopTime)
        //    {
        //        StopPercent = stopTimeIncrement * stopTimeMultiplier;
        //    }
        //}
        //else if (Starting)
        //{
        //    // increase the start time
        //    startTimeIncrement += Time.deltaTime;
        //    if (startTimeIncrement < StartTime)
        //    {
        //        StartPercent = startTimeIncrement * startTimeMultiplier;
        //    }
        //    else
        //    {
        //        Starting = false;
        //    }
        //}
        //else if (Duration <= 0.0f)
        //{
        //    // time to stop, no duration left
        //    Stop();
        //}
        if (isFlameActive)
        {
            RaycastHit[] hits = Physics.SphereCastAll(new Ray(transform.position, transform.TransformDirection(Vector3.forward)), fireRadius, fireDistance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.yellow);
            foreach (RaycastHit hit in hits)
            {
                PlayerController playerController = hit.collider.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(fireDamage);
                }
            }
        }
    }

    public static void CreateExplosion(Vector3 pos, float radius, float force)
    {
        if (force <= 0.0f || radius <= 0.0f)
        {
            return;
        }

        // find all colliders and add explosive force
        Collider[] objects = UnityEngine.Physics.OverlapSphere(pos, radius);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(force, pos, radius);
            }
        }
    }

    public virtual void Stop()
    {
        if (Stopping)
        {
            return;
        }
        Stopping = true;

        // cleanup particle systems
        RPC_StopParticle();

        //StartCoroutine(CleanupEverythingCoRoutine());
    }

    private void StopParticleSystems()
    {
        RPC_StopParticle();
    }

    public bool Starting
    {
        get;
        private set;
    }

    public float StartPercent
    {
        get;
        private set;
    }

    public bool Stopping
    {
        get;
        private set;
    }

    public float StopPercent
    {
        get;
        private set;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayParticle()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
        if (AudioSource != null)
        {
            AudioSource.Play();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StopParticle()
    {
        foreach (var particle in particles)
        {
            particle.Stop();
        }
        if (AudioSource != null)
        {
            AudioSource.Stop();
        }
    }
}