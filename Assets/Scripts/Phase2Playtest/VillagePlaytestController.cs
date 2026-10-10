using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace MedievalVillageDefense.ProjectBootstrap
{
    /// <summary>
    /// Deliberately small, input-system-independent phone playtest loop.  It uses
    /// the legacy pointer API because WebGL browsers translate a tap into the same
    /// pointer event as a mouse click.
    /// </summary>
    public sealed class VillagePlaytestController : MonoBehaviour
    {
        private const int StartingCurrency = 20;
        private const int TowerCost = 10;
        private const int StartingHealth = 10;
        private readonly List<PlaytestEnemy> enemies = new List<PlaytestEnemy>();
        private Transform spawn;
        private Transform goal;
        private VillagePlacementZone[] zones;
        private int currency = StartingCurrency;
        private int villageHealth = StartingHealth;
        private bool waveRunning;
        private float nextSpawn;
        private int spawned;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            spawn = GameObject.Find("EnemySpawn").transform;
            goal = GameObject.Find("VillageGoal").transform;
            zones = FindObjectsByType<VillagePlacementZone>(FindObjectsSortMode.None);
            CreateStyles();
        }

        private void Update()
        {
            if (waveRunning && villageHealth > 0 && spawned < 8 && Time.time >= nextSpawn)
            {
                SpawnEnemy();
                spawned++;
                nextSpawn = Time.time + 2.25f;
            }

            if (PointerPressed()) TryPlaceTower(PointerPosition());
            enemies.RemoveAll(enemy => enemy == null);
        }

        private void OnGUI()
        {
            if (labelStyle == null) CreateStyles();
            var scale = Mathf.Max(1f, Screen.width / 900f);
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            var width = Screen.width / scale;
            GUI.Box(new Rect(18, 18, width - 36, 108), "");
            GUI.Label(new Rect(38, 30, width - 76, 32), $"Village Health: {villageHealth}    Gold: {currency}", labelStyle);
            GUI.Label(new Rect(38, 62, width - 76, 28), "Tap a blue tower zone to place one ballista. Enemies follow the road to the village.", labelStyle);
            if (!waveRunning && GUI.Button(new Rect(38, 92, 180, 48), "Start wave", buttonStyle)) StartWave();
            if (GUI.Button(new Rect(width - 218, 92, 180, 48), "Restart", buttonStyle)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (villageHealth <= 0) GUI.Label(new Rect(width * .35f, 170, 300, 60), "Village lost — tap Restart", labelStyle);
            GUI.matrix = Matrix4x4.identity;
        }

        private void StartWave()
        {
            waveRunning = true;
            nextSpawn = Time.time + .5f;
        }

        private void SpawnEnemy()
        {
            if (!NavMesh.SamplePosition(spawn.position, out var hit, 3f, NavMesh.AllAreas)) return;
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Raiders (playtest)";
            enemy.transform.position = hit.position + Vector3.up;
            enemy.GetComponent<Renderer>().material.color = new Color(.45f, .08f, .06f);
            var agent = enemy.AddComponent<NavMeshAgent>();
            agent.radius = .35f;
            agent.height = 1.8f;
            agent.speed = 3.2f;
            agent.acceleration = 12f;
            var behaviour = enemy.AddComponent<PlaytestEnemy>();
            behaviour.Initialize(this, goal.position);
            enemies.Add(behaviour);
        }

        private void TryPlaceTower(Vector2 pointer)
        {
            if (currency < TowerCost || villageHealth <= 0) return;
            var ray = Camera.main.ScreenPointToRay(pointer);
            if (!UnityEngine.Physics.Raycast(ray, out var hit, 200f)) return;
            var zone = hit.collider.GetComponent<VillagePlacementZone>();
            if (zone == null || zone.IsOccupied) return;
            zone.IsOccupied = true;
            currency -= TowerCost;
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "Ballista Defense (playtest)";
            tower.transform.position = zone.transform.position + Vector3.up * .75f;
            tower.transform.localScale = new Vector3(1.25f, 1.5f, 1.25f);
            tower.GetComponent<Renderer>().material.color = new Color(.32f, .18f, .07f);
            tower.AddComponent<PlaytestTower>().Initialize(this);
        }

        internal PlaytestEnemy FindTarget(Vector3 position, float range)
        {
            PlaytestEnemy closest = null;
            var closestDistance = range * range;
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                var distance = (enemy.transform.position - position).sqrMagnitude;
                if (distance < closestDistance) { closest = enemy; closestDistance = distance; }
            }
            return closest;
        }

        internal void AwardKill(PlaytestEnemy enemy)
        {
            currency += 5;
            enemies.Remove(enemy);
        }

        internal void DamageVillage(PlaytestEnemy enemy)
        {
            villageHealth = Mathf.Max(0, villageHealth - 1);
            enemies.Remove(enemy);
        }

        private static bool PointerPressed() => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        private static Vector2 PointerPosition() => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        private void CreateStyles()
        {
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, wordWrap = true, normal = { textColor = Color.white } };
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 20 };
        }
    }

    public sealed class VillagePlacementZone : MonoBehaviour { public bool IsOccupied { get; set; } }

    public sealed class PlaytestTower : MonoBehaviour
    {
        private VillagePlaytestController controller;
        private float nextShot;
        internal void Initialize(VillagePlaytestController value) => controller = value;
        private void Update()
        {
            if (Time.time < nextShot) return;
            var target = controller.FindTarget(transform.position, 11f);
            if (target == null) return;
            target.TakeDamage();
            nextShot = Time.time + .75f;
        }
    }

    public sealed class PlaytestEnemy : MonoBehaviour
    {
        private VillagePlaytestController controller;
        private NavMeshAgent agent;
        private Vector3 goal;
        private int health = 3;
        internal void Initialize(VillagePlaytestController value, Vector3 destination)
        {
            controller = value;
            agent = GetComponent<NavMeshAgent>();
            goal = destination;
            agent.SetDestination(goal);
        }
        private void Update()
        {
            if (agent != null && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 1.3f)
            {
                controller.DamageVillage(this);
                Destroy(gameObject);
            }
        }
        internal void TakeDamage()
        {
            if (--health > 0) return;
            controller.AwardKill(this);
            Destroy(gameObject);
        }
    }
}
