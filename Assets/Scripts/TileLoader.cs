using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TileLoader : MonoBehaviour
{
    public GameObject[] tilePrefab;
    public GameObject parent;
    public GameObject boundPrefab; // Bound 프리팹을 인스펙터에서 할당
    public TextAsset csvFile;      // 인스펙터에서 할당할 수 있도록 TextAsset 타입으로 선언
    public int boundDistance = 3;

    public Camera mainCamera;
    public float size = 5.0f; // 기본 값

    public GameObject player;
    public GameObject goal;

    public TextDisplay TD;


    // 레이저와 버튼을 연결하기 위한 리스트
    private Dictionary<int, List<GameObject>> laserList = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, GameObject> buttonList = new Dictionary<int, GameObject>();

    void Start()
    {
        LoadTilesFromCSV();
        TD.StageUpdate();
        AddBounds();
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = size;
            mainCamera.transform.position = new Vector3(size - 4f, 0, -10);
        }
    }

    void LoadTilesFromCSV()
    {
        buttonList.Clear();
        laserList.Clear();
        for (int i = 0; i < 10; i++)
        {
            laserList.Add(i, new List<GameObject>());
        }

        csvFile = MenuManager.MapFile;
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 할당되지 않았습니다.");
            return;
        }

        // CSV 파일 읽기
        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 중앙 좌표 계산
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        size = lines.Length / 2 + 2;
        float startX = -cols / 2.0f + 0.5f;
        float startY = rows / 2.0f - 0.5f;

        // CSV 데이터를 파싱하여 타일 배치
        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                string tileType = values[x];
                Vector2 position = new Vector2(startX + x, startY - y);

                GenerateTile(tileType, position);
            }
        }
        Debug.Log("1");
        ConnectLaser();
    }

    void AddBounds()
    {
        if (boundPrefab == null)
        {
            Debug.LogError("Bound 프리팹이 할당되지 않았습니다.");
            return;
        }

        // CSV 파일 읽기
        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 중앙 좌표 계산
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        float startX = -cols / 2.0f + 0.5f;
        float startY = rows / 2.0f - 0.5f;

        // 테두리 추가
        for (int x = -boundDistance; x <= cols + boundDistance - 1; x++)
        {
            // 상단 테두리
            Vector2 topPosition = new Vector2(startX + x, startY + boundDistance);
            Instantiate(boundPrefab, topPosition, Quaternion.identity, parent.transform);

            // 하단 테두리
            Vector2 bottomPosition = new Vector2(startX + x, startY - rows - boundDistance + 1);
            Instantiate(boundPrefab, bottomPosition, Quaternion.identity, parent.transform);
        }

        for (int y = -boundDistance; y < rows + boundDistance; y++)
        {
            // 좌측 테두리
            Vector2 leftPosition = new Vector2(startX - boundDistance, startY - y);
            Instantiate(boundPrefab, leftPosition, Quaternion.identity, parent.transform);

            // 우측 테두리
            Vector2 rightPosition = new Vector2(startX + cols + boundDistance - 1, startY - y);
            Instantiate(boundPrefab, rightPosition, Quaternion.identity, parent.transform);
        }
    }

    void GenerateTile(string tileType, Vector2 pos)
    {
        int typeNumber = int.Parse(tileType[0].ToString()) - 1;

        switch (tileType[0])
        {
            case '1':
            case '5':
                // [1] 벽과 [5] 시간 정지 불가능 지역
                Instantiate(tilePrefab[typeNumber], pos, Quaternion.identity, parent.transform);
                break;

                // [2] 장애물 - 색
            case '2':
                GameObject temp1 = Instantiate(tilePrefab[typeNumber], pos, Quaternion.identity, parent.transform);
                temp1.GetComponent<ColorPlatform>().type = tileType[1] switch
                {
                    'R' => "red",
                    'G' => "green",
                    'B' => "blue",
                    _ => "none"
                };
                break;

            case '3':
                // 레이저 버튼
                GameObject temp2 = Instantiate(tilePrefab[typeNumber], pos, Quaternion.identity, parent.transform);
                int id1 = int.Parse(tileType[1].ToString());

                temp2.GetOrAddComponent<Button>().id = id1;
                buttonList.Add(id1, temp2);

                break;

            case '4':
                // 레이저 사출기
                GameObject temp4 = Instantiate(tilePrefab[typeNumber], pos, Quaternion.identity, parent.transform);
                temp4.GetOrAddComponent<Laser>().type = tileType[1] switch
                {
                    '1' => "down",
                    '2' => "up",
                    '3' => "right",
                    '4' => "left",
                    _ => "none"
                };
                int id2 = int.Parse(tileType[3].ToString());

                temp4.GetOrAddComponent<Laser>().id = id2;


                temp4.GetOrAddComponent<Laser>().State = tileType[2] == 'P';


                laserList[id2].Add(temp4);

                break;

            case '6':
                int detailType = int.Parse(tileType[1].ToString());
                Instantiate(tilePrefab[typeNumber + detailType - 1], pos, Quaternion.identity, parent.transform);
                break;

            case '7':
                GameObject temp3 = Instantiate(tilePrefab[typeNumber + 1], pos, Quaternion.identity, parent.transform);
                temp3.GetComponent<ColorZone>().type = tileType[1] switch
                {
                    'R' => "red",
                    'G' => "green",
                    'B' => "blue",
                    _ => "none"
                };

                temp3.GetComponent<ColorZone>().Initialize();

                break;
            case '8':
                player.transform.position = pos;
                player.transform.position += new Vector3(0, 0, -2);
                break;
            case '9':
                goal.transform.position = pos;
                goal.transform.position += new Vector3(0, 0, -2);
                break;
        }
    }

    void ConnectLaser()
    {

        foreach (var btn in buttonList)
        {

            int id = btn.Key;

            btn.Value.GetComponent<Button>().RegisterLaser(laserList[id]);

            foreach (GameObject go in laserList[id])
            {
                go.GetComponent<Laser>().Initialize();
            }

            Debug.Log($"버튼 id : {id} 에는 {laserList[id].Count}개의 레이저가 포함됩니다.");
        }
    }
}
