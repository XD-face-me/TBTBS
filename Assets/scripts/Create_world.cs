﻿using System.Collections.Generic;
using Assets.scripts.tile;
using UnityEngine;


namespace Assets.scripts
{
    public enum Player { Blue, Red };
    public class Create_world : MonoBehaviour
    {
        Unit_manager unit_manager = new Unit_manager();
        Tile_manager tile_manager = new Tile_manager();
        Input_manager input_manager;

        public int turns;
        public GameObject focus;
        public Animator focus_an;
        public Texture texture;
        public UnityEngine.UI.Text movespeed;
        public UnityEngine.UI.Text strength;
        public UnityEngine.UI.Text type;
        private Player current_player = Player.Blue;
        void Start()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            input_manager = new Input_manager(focus, focus_an);
            Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;
            
            add_tiles(ref tile_manager);

            Vector3[] vertices = tile_manager.get_vertices();
            List<int> tri = tile_manager.get_tri(vertices);
            Vector2[] uv = tile_manager.get_uv();
            
            mesh.vertices = vertices;
            mesh.triangles = tri.ToArray();
            mesh.uv = uv;
            //mesh.Optimize();
            mesh.RecalculateNormals();
            GetComponent<Renderer>().material.SetColor("_Color", new Color(.7f, .7f, .7f));
            GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);

            unit_manager.add<unit.Test>(tile_manager[0], Player.Blue);
            unit_manager.add<unit.Test>(tile_manager[8], Player.Red);

            Debug.Log("Loaded world in " + sw.ElapsedMilliseconds + " ms");
            sw.Stop();
            end_turn();
            
        }


        void Update()
        {
            input_manager.process_input(ref unit_manager, ref tile_manager, current_player);
            
            unit_manager.move_units();      
        }

        public void switch_player(GameObject button)
        {
            
            if (current_player == Player.Blue)
            {
                current_player = Player.Red;
                button.GetComponent<UnityEngine.UI.Image>().color = reference.Player_color.red;
                Debug.Log("switched to red");
            }
            else
            {
                current_player = Player.Blue;
                button.GetComponent<UnityEngine.UI.Image>().color = reference.Player_color.blue;
                Debug.Log("switched to blue");
            }
        }

        public void end_turn()
        {
            turns++;
            for(int i = 0; i < unit_manager.count; i++)
            {
                unit_manager[i].can_move = true;
                unit_manager[i].can_attack = true;
            }
            Debug.Log("turn " + turns + " has been ended");
        }

        private void add_tiles(ref Tile_manager tile_manager)
        {
            string[,] level = CSVReader.SplitCsvGrid(Resources.Load<TextAsset>("levels/level1").text);
            for (int i = 1; i < 40 - 1; i++)
            {
                int x = int.Parse(level[1, i]);
                float height = float.Parse(level[2, i]);
                int z = int.Parse(level[3, i]);

                switch(level[0,i])
                {
                    case "Grassland":
                        tile_manager.add<Grassland>(x, height, z);
                        break;
                    case "Stone":
                        tile_manager.add<Stone>(x, height, z);
                        break;
                    case "Desert":
                        tile_manager.add<Desert>(x, height, z);
                        break;
                }
                

                //System.Type type = System.Type.GetType(level[i, 0]);

                //System.Action<> GenMethod = GenericMethod<Grassland>;
                //MethodInfo method = GetType().GetMethod("add_tiles");
                //MethodInfo generic = method.MakeGenericMethod(type);
                //generic.Invoke(this, new object[] { x, height, z });
            }
        }
    }
}