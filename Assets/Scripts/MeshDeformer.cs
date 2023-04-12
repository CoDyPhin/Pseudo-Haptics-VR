using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

	public float springForce = 20f;
	public float damping = 5f;

	Mesh deformingMesh;
	MeshFilter meshFilter;
	Vector3[] originalVertices, displacedVertices;
	Vector3[] vertexVelocities;
	[SerializeField] MeshCollider col;
	float uniformScale = 1f;
	bool alreadyReset = false;
	bool changed = false;
	int colliderUpdateTreshold = 20;
	Vector3 lastHitPoint = Vector3.zero;

	void Start () {
		if(damping <= 0) damping = 1;
		if(springForce <= 0) springForce = 1;
		meshFilter = GetComponent<MeshFilter>();
		deformingMesh = meshFilter.mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}
		vertexVelocities = new Vector3[originalVertices.Length];
	}


	void FixedUpdate () {
		int changedVertexCount = 0;
		uniformScale = transform.localScale.x;
		if(!alreadyReset && !changed){
			deformingMesh.vertices = originalVertices;
			deformingMesh.RecalculateNormals();
			col.sharedMesh = deformingMesh;
			//Debug.Log("Reseted collider");
			alreadyReset = true;
		}
		for (int i = 0; i < displacedVertices.Length; i++) {
			UpdateVertex(i);
			if(Vector3.Distance(displacedVertices[i], originalVertices[i]) >= 0.001f){
				changedVertexCount++;
			}
		}
		if(changedVertexCount >= 0.10f*displacedVertices.Length){
			deformingMesh.vertices = displacedVertices;
			deformingMesh.RecalculateNormals();
			if(colliderUpdateTreshold > 0){
				colliderUpdateTreshold--;
				changedVertexCount = 0;
				return;
			}
			colliderUpdateTreshold = 20;
			col.sharedMesh = deformingMesh;
			changed = true;
			alreadyReset = false;
		}
		else{
			changed = false;
		}
		changedVertexCount = 0;
	}

	void UpdateVertex (int i) {
		Vector3 velocity = vertexVelocities[i];
		Vector3 displacement = displacedVertices[i] - originalVertices[i];
		displacement *= uniformScale;
		velocity -= displacement * springForce * Time.deltaTime;
		float resistence = damping * Time.deltaTime;
		if(resistence > 1f) resistence = 1f;
		velocity *= 1f - resistence;
		vertexVelocities[i] = velocity;
		displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
	}

	public void AddDeformingForce (Vector3 point, float force) {
		point = transform.InverseTransformPoint(point);
		//Debug.Log("After inverse transform: " + point);
		for (int i = 0; i < displacedVertices.Length; i++) {
			AddForceToVertex(i, point, force);
		}
	}

	void AddForceToVertex (int i, Vector3 point, float force) {
		Vector3 pointToVertex = displacedVertices[i] - point;
		pointToVertex *= uniformScale;
		float attenuatedForce = -force / (1f + pointToVertex.sqrMagnitude);
		if(Mathf.Abs(attenuatedForce) < 0.01f)
			attenuatedForce = 0;
		float velocity = attenuatedForce * Time.deltaTime;
		vertexVelocities[i] += pointToVertex.normalized * velocity;
	}
}