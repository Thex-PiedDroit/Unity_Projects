
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;



public class FollowTargetBehavior : MonoBehaviour
{
	#region Variables (public)

	public float fSpeed = 5.0f;

	#endregion

	#region Variables (private)

	GameObject pPlayerObject;
	GameObject tlastPlanet;
	Vector3 tLastPlanetHitPoint;

	float fRespawnRotationDist = 1.0f;

	private LoadQuitGame tLoadQuitGameScript;

    #endregion
	

	void Awake ()
	{
		pPlayerObject = transform.parent.gameObject;

		tLoadQuitGameScript = GameObject.Find("Main Camera").GetComponent<LoadQuitGame>();
	}


	void Start()
	{
		transform.parent = GameObject.Find(pPlayerObject.name + "_Start").transform.FindChild("Orbit");
	}


	void FixedUpdate()
	{
		
	}


	void Update ()
	{
		if (transform.parent)
		{
			switch(transform.parent.tag)
			{
			case "Orbit":
				{
					if (Input.GetButtonDown(pPlayerObject.name + "_Jump"))
					{
						transform.parent = pPlayerObject.transform.FindChild("Player");
						transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
						transform.parent = pPlayerObject.transform;
					}

					break;
				}

			case "BlackHoleOrbit":
				{
					BlackHoleBehavior tBlackHoleScript = transform.parent.parent.gameObject.GetComponent<BlackHoleBehavior>();
					tBlackHoleScript.OrbitUpdate(gameObject);

					break;
				}

			case "Squid":
				{
					SquidBehavior tSquidScript = transform.parent.FindChild("Squid").gameObject.GetComponent<SquidBehavior>();
					tSquidScript.Eat(gameObject.GetComponent<FollowTargetBehavior>());

					break;
				}

			default:
				{
					transform.Translate(new Vector3(0.0f, 0.0f, fSpeed) * Time.deltaTime * tLoadQuitGameScript.TimeScale, Space.Self);

					break;
				}
			}
		}
	}


	void OnTriggerEnter2D(Collider2D tCollision)
	{
		switch (tCollision.gameObject.tag)
		{
		case "Planet":
			{
				Transform tOrbit = tCollision.GetComponent<PlanetBehavior>().transform.parent.FindChild("Orbit");
				transform.parent = tOrbit;
				tlastPlanet = tOrbit.gameObject;

				Vector3 tPlanetToSelf = transform.position - tCollision.gameObject.transform.position;
				float fOrbitDist = (tCollision.GetComponent<CircleCollider2D>().radius * tCollision.transform.localScale.x) * tCollision.transform.parent.localScale.x;
				Vector3 tNewPos = tCollision.gameObject.transform.position + (tPlanetToSelf.normalized * fOrbitDist);

				tLastPlanetHitPoint = tNewPos;
				transform.position = tNewPos;

				break;
			}

		case "BlackHoleGrab":
			{
				Transform tOrbit = tCollision.transform.FindChild("Orbit");
				transform.parent = tOrbit;

				Vector3 tBlackHoleToSelf = transform.position - tCollision.GetComponent<CircleCollider2D>().transform.position;
				float fOrbitDist = (tCollision.GetComponent<CircleCollider2D>().radius * tCollision.transform.localScale.x) * tCollision.transform.parent.localScale.x;
				Vector3 tNewPos = tCollision.GetComponent<CircleCollider2D>().transform.position + (tBlackHoleToSelf.normalized * fOrbitDist);

				transform.position = tNewPos;

				break;
			}

		case "Squid":
			{
				if (transform.parent.tag != "Squid")
				{
					transform.parent = tCollision.gameObject.transform.parent;
					pPlayerObject.transform.parent = tCollision.gameObject.transform.FindChild("ShipPos");
					pPlayerObject.transform.localPosition = Vector3.zero;
					pPlayerObject.transform.FindChild("Player").localPosition = Vector3.zero;
					pPlayerObject.transform.FindChild("Player").GetComponent<CharacController>().Freeze = true;
					transform.parent.FindChild("Squid").gameObject.GetComponent<SquidBehavior>().Eating();
				}

				break;
			}
		}
	}


	public void Respawn()
	{
		transform.position = tLastPlanetHitPoint;
		transform.parent = tlastPlanet.transform;

		float fRotationDirection = -(tlastPlanet.GetComponent<Rotator>().RotationDirection);

		float fPerimeter = (2.0f * Mathf.PI) * (tlastPlanet.transform.localScale.x * tlastPlanet.transform.parent.localScale.x);
		float fAngle = fRespawnRotationDist / fPerimeter;
		fAngle *= 360.0f;
		fAngle *= fRotationDirection;

		pPlayerObject.transform.parent = GameObject.Find("Scene").transform;

		pPlayerObject.transform.FindChild("Player").position = tLastPlanetHitPoint;
		pPlayerObject.transform.FindChild("Player").Rotate(tlastPlanet.transform.position, fAngle);

		pPlayerObject.transform.FindChild("Player").GetComponent<CharacController>().Freeze = false;
	}


	void OnCollisionEnter2D(Collision2D tCollision)
	{
		if (tCollision.gameObject.tag == "MapBorder")
		{
			Vector3 tNormal = tCollision.contacts[0].normal;

			float fDotOnNormal = Vector3.Dot(transform.forward, -tNormal);
			float fIncidenceAngle = Mathf.Acos(fDotOnNormal) * Mathf.Rad2Deg;
			float fReflectedAngle = (90.0f - fIncidenceAngle) * 2.0f;
			fReflectedAngle *= Mathf.Deg2Rad;

			Vector3 tRotatedForward = new Vector3(-transform.forward.y, transform.forward.x);	// Rotation 90° of forward vector

			if (Vector3.Dot(-tNormal, tRotatedForward) < 0.0f)
				fReflectedAngle *= -1.0f;

			float fSin = Mathf.Sin(-fReflectedAngle);
			float fCos = Mathf.Cos(-fReflectedAngle);
			Vector3 tNewForward = new Vector3((transform.forward.x * fCos) - (transform.forward.y * fSin), (transform.forward.x * fSin) + (transform.forward.y * fCos), 0.0f);
			transform.forward = tNewForward;
		}
	}


	public bool OnOrbit
	{
		get { return transform.parent.tag != "PlayerContainer"; }
	}

	public Vector3 OrbitPlanetPos
	{
		get { return transform.parent.position; }
	}

	public string PlayerName
	{
		get { return pPlayerObject.name; }
	}
}
