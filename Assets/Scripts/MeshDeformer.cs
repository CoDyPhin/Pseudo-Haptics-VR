using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	[SerializeField][Range(0f,1f)] float percentageTreshhold = 0.8f;
	[SerializeField] int vertexGroups = 50;

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
		//if(percentageTreshhold < 0) percentageTreshhold = 0;
		//if(percentageTreshhold > 100) percentageTreshhold = 100;
	}


	void FixedUpdate () {
		//int changedVertexCount = 0;
		uniformScale = transform.localScale.x;
		for (int i = 0; i < displacedVertices.Length; i++) {
			UpdateVertex(i);
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
		col.sharedMesh = deformingMesh;
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
		List<int> sortedVertices = DivideVertexesIntoGroups(point);
		for (int i = 0; i < (percentageTreshhold * sortedVertices.Count); i++) {
			AddForceToVertex(sortedVertices[i], point, force, ((i * vertexGroups) / sortedVertices.Count)+1);
		}
	}

	void AddForceToVertex (int i, Vector3 point, float force, int group) {
		Vector3 pointToVertex = displacedVertices[i] - point;
		pointToVertex *= uniformScale;
		float attenuatedForce = -force / (1f + pointToVertex.sqrMagnitude);
		/*if(Mathf.Abs(attenuatedForce) < 0.01f)
			attenuatedForce = 0;*/
		float velocity = attenuatedForce * Time.deltaTime;
		vertexVelocities[i] += pointToVertex.normalized * velocity;
	}

	
	List<int> DivideVertexesIntoGroups(Vector3 hitpoint){
		Dictionary<int, float> vertices = new Dictionary<int, float>();
		for (int i = 0; i < displacedVertices.Length; i++) {
			Vector3 pointToVertex = displacedVertices[i] - hitpoint;
			pointToVertex *= uniformScale;
			float distance = pointToVertex.sqrMagnitude;
			vertices.Add(i, distance);
		}
		// get a list of the keys sorted by ascending values
		List<int> sortedKeys = new List<int>(vertices.Keys);
		sortedKeys.Sort(delegate(int a, int b) {
			return vertices[a].CompareTo(vertices[b]);
		});
		return sortedKeys;
	}

}