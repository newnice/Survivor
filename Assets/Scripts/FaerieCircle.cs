using Nightmare;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct FaerieMood {
    public Color mainFaerieColor;
    public Color mainCircleColor;
    public Color accentColor;
    public Color glowColor;
    public float glowIntensity;
    public float areaCost;
    public float windForce;
    public float speed;
    public float minimumTime;
}

public class FaerieCircle : MonoBehaviour {
    public FaerieMood happyFaerie;
    public FaerieMood angryFaerie;
    [SerializeField] private int grenadeStock = 1;
    [SerializeField] private float cullRadius = 5f;
    
    private float faerieSpeed;
    private float radius = 1f;
    private ParticleSystem faerieParticles;
    private ParticleSystem circleParticles;
    private WindZone windZone;
    private int remainingGrenades;
    private Transform faerie;
    private Light faerieGlow;
    private Vector3 moveVector = Vector3.zero;
    public float moveTimer = 0f;
    private CullingGroup cullGroup;

    void Start() {
        PopulateParticleSystemCache();
        SetupStateBehaviours();
        SetupWind();
        SetupCullingGroup();

        faerieGlow = GetComponentInChildren<Light>();

        remainingGrenades = grenadeStock;
        faerieSpeed = happyFaerie.speed;
    }

    private void SetupStateBehaviours() {
        var fa = FindObjectOfType<FaerieAnger>();
        var anim = gameObject.GetComponent<Animator>();
        var stateBehaviours = anim.GetBehaviours<FaerieStateBehaviour>();
        foreach (var state in stateBehaviours) {
            state.Setup(this, fa);
        }
    }

    private void PopulateParticleSystemCache() {
        var pSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var pSystem in pSystems) {
            if (pSystem.shape.shapeType == ParticleSystemShapeType.Circle) {
                circleParticles = pSystem;
                radius = pSystem.shape.radius;
            }
            else {
                faerie = pSystem.gameObject.transform;
                faerieParticles = pSystem;
            }
        }
    }

    private void SetupWind() {
        windZone = GetComponentInChildren<WindZone>();
        windZone.windMain = happyFaerie.windForce;
    }

    private void SetupCullingGroup() {
        cullGroup = new CullingGroup {targetCamera = Camera.main};
        cullGroup.SetBoundingSpheres(new[] {new BoundingSphere(transform.position, cullRadius)});
        cullGroup.SetBoundingSphereCount(1);
        cullGroup.onStateChanged += OnStateChanged;
    }

    void OnStateChanged(CullingGroupEvent cullEvent) {
        if (cullEvent.isVisible) {
            faerieParticles.Play(true);
        }
        else {
            faerieParticles.Pause();
        }
    }

    void OnTriggerExit(Collider coll) {
        if (coll.CompareTag(TagNames.Enemy) && coll.attachedRigidbody.isKinematic) {
            MakeAngry();
        }
    }

    void Update() {
        if (moveTimer > 0f) {
            moveTimer -= Time.deltaTime;
            MoveFaerie(Time.deltaTime * moveVector);
        }
        else {
            moveTimer = faerieSpeed;
            moveVector = GetRandomVector();
        }
    }

    private void ActivateFaerie(bool activate) {
        var faerieGo = faerie.gameObject;
        if (faerieGo.activeInHierarchy != activate) {
            faerieGo.SetActive(activate);
        }
    }

    public void SetMood(bool angry) {
        if (angry) {
            SetValuesFromMood(angryFaerie);
        }
        else {
            SpawnGrenade();
            SetValuesFromMood(happyFaerie);
        }
    }

    private void SetValuesFromMood(FaerieMood mood) {
        faerieSpeed = mood.speed;

        ColorParticle(faerieParticles, mood.mainFaerieColor, mood.accentColor);
        ColorParticle(circleParticles, mood.mainCircleColor, mood.accentColor);

        faerieGlow.color = mood.glowColor;
        faerieGlow.intensity = mood.glowIntensity;

        windZone.windMain = mood.windForce;

        NavMesh.SetAreaCost(NavMesh.GetAreaFromName("FaerieCircle"), mood.areaCost);
    }

    private void ColorParticle(ParticleSystem pSys, Color mainColor, Color accentColor) {
        var pMain = pSys.main;
        pMain.startColor = new ParticleSystem.MinMaxGradient(mainColor, accentColor);
    }

    private void SpawnGrenade() {
        if (remainingGrenades < 1) {
            return;
        }

        remainingGrenades--;
        PoolManager.SharedInstance.Pull("Grenade", transform.position, Quaternion.identity);
    }

    private void MakeAngry() {
        GetComponent<Animator>().SetInteger(AnimationConstants.AngerAttribute, 11);
    }

    private void MoveFaerie(Vector3 delta) {
        faerie.localPosition += delta;
    }

    private Vector3 GetRandomVector() {
        var randomPoint = Random.insideUnitSphere * radius;
        randomPoint += radius * Vector3.up;
        return (randomPoint - faerie.localPosition) / faerieSpeed;
    }

    void OnDestroy() {
        cullGroup?.Dispose();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cullRadius);
    }
}