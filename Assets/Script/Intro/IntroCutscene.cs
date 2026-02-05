using UnityEngine;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    [Header("--- CẤU HÌNH TEST ---")]
    public bool isTesting = true;

    [Header("--- CAMERA & MAP (QUAN TRỌNG) ---")]
    public Camera mainCam;                
    public Collider2D mapBounds;          
    public float smoothTime = 0.2f;       
    [Header("--- ZOOM SETTING ---")]
    public float zoomSize = 2.5f;         
    public float normalSize = 5f;         
    public float zoomSpeed = 2f;

    [Header("--- DIỄN VIÊN & ĐẠO CỤ ---")]
    public Transform player;              
    public Animator playerAnim;           
    public MonoBehaviour playerMovementScript; 
    public GameObject seedInHand;         
    public GameObject seedPrefab;         
    public Transform handPos;             
    public Transform potTarget;           
    public float walkSpeed = 2.5f;        

    private Vector3 _currentVelocity;     // Biến phụ cho hàm SmoothDamp
    private bool _introFinished = false;  // Biến cờ: Intro xong chưa?

    void Start()
    {
        // 1. Setup ban đầu
        if (seedInHand) seedInHand.SetActive(false);
        if (playerMovementScript) playerMovementScript.enabled = false;

        // Teleport Camera đến ngay chỗ Player lúc đầu để không bị trượt từ xa tới
        if (mainCam != null && player != null)
        {
            Vector3 startPos = player.position;
            startPos.z = -10f; // Luôn giữ Z = -10 cho 2D
            mainCam.transform.position = startPos;
        }

        // 2. Quyết định chạy Intro hay không
        bool firstTime = PlayerPrefs.GetInt("IntroPlayed", 0) == 0;
        if (isTesting || firstTime)
        {
            StartCoroutine(RunIntro());
        }
        else
        {
            // Bỏ qua Intro -> Vào trạng thái Gameplay luôn
            EndIntroImmediate();
        }
    }

    // --- HÀM CAMERA FOLLOW + LIMIT (CHẠY LIÊN TỤC) ---
    void LateUpdate()
    {
        if (mainCam == null || player == null) return;

        // 1. Tính vị trí mục tiêu (Lấy vị trí Player)
        Vector3 targetPosition = player.position;
        targetPosition.z = -10f; // Giữ nguyên Z camera

        // 2. XỬ LÝ GIỚI HẠN MAP (CLAMP)
        if (mapBounds != null)
        {
            // Tính toán chiều cao và chiều rộng của Camera dựa trên mức Zoom hiện tại
            float camHeight = mainCam.orthographicSize;
            float camWidth = camHeight * mainCam.aspect;

            // Tính toán giới hạn (Bounds)
            float minX = mapBounds.bounds.min.x + camWidth;
            float maxX = mapBounds.bounds.max.x - camWidth;
            float minY = mapBounds.bounds.min.y + camHeight;
            float maxY = mapBounds.bounds.max.y - camHeight;

            // Kẹp vị trí Camera nằm trong giới hạn này
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // 3. Di chuyển Camera mượt mà tới vị trí đã tính
        // Dùng SmoothDamp để camera chạy mượt như Cinemachine
        mainCam.transform.position = Vector3.SmoothDamp(
            mainCam.transform.position, 
            targetPosition, 
            ref _currentVelocity, 
            smoothTime
        );
    }

    IEnumerator RunIntro()
    {
        // --- GIAI ĐOẠN 1: ZOOM VÀO ---
        StartCoroutine(DoZoom(zoomSize)); 
        yield return new WaitForSeconds(1f);

        // --- GIAI ĐOẠN 2: TỰ ĐI ---
        playerAnim.SetFloat("Speed", 1f); // Bật anim chạy (giả sử param tên là Speed)
        
        while (Vector3.Distance(player.position, potTarget.position) > 0.05f)
        {
            // Tính hướng & Di chuyển
            Vector3 direction = (potTarget.position - player.position).normalized;
            playerAnim.SetFloat("Horizontal", direction.x);
            playerAnim.SetFloat("Vertical", direction.y);
            
            player.position = Vector3.MoveTowards(player.position, potTarget.position, walkSpeed * Time.deltaTime);
            yield return null;
        }
        playerAnim.SetFloat("Speed", 0f); // Dừng anim
        yield return new WaitForSeconds(0.5f);

        // --- GIAI ĐOẠN 3: THẢ HẠT ---
        if (seedInHand) seedInHand.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(DropSeedAnimation());
        if (seedInHand) seedInHand.SetActive(false);
        yield return new WaitForSeconds(1f);

        // --- GIAI ĐOẠN 4: ZOOM RA ---
        Debug.Log("🎥 Zoom out về Gameplay...");
        yield return StartCoroutine(DoZoom(normalSize)); // Đợi zoom xong hẳn
        
        // --- KẾT THÚC ---
        EndIntroImmediate();
        
        if (!isTesting)
        {
            PlayerPrefs.SetInt("IntroPlayed", 1);
            PlayerPrefs.Save();
        }
    }

    void EndIntroImmediate()
    {
        _introFinished = true;
        
        // Đảm bảo Zoom đúng kích thước chuẩn
        mainCam.orthographicSize = normalSize;
        
        // Mở khóa điều khiển cho người chơi
        if (playerMovementScript) playerMovementScript.enabled = true;
        
        // Kích hoạt cốt truyện nếu có
        // if (GameController.Instance != null) GameController.Instance.StartChapter();
    }

    // Hàm Zoom thủ công cho Main Camera
    IEnumerator DoZoom(float targetSize)
    {
        float startSize = mainCam.orthographicSize;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            mainCam.orthographicSize = Mathf.Lerp(startSize, targetSize, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        mainCam.orthographicSize = targetSize;
    }

    IEnumerator DropSeedAnimation()
    {
        // (Giữ nguyên code rơi hạt giống cũ của bạn ở đây)
        GameObject fallingSeed = Instantiate(seedPrefab, handPos.position, Quaternion.identity);
        Vector3 startP = handPos.position;
        Vector3 endP = potTarget.position;
        float duration = 0.6f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 currentPos = Vector3.Lerp(startP, endP, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * 0.5f; 
            fallingSeed.transform.position = currentPos;
            yield return null;
        }
        Destroy(fallingSeed); 
    }
}