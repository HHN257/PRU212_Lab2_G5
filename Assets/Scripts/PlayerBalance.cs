using UnityEngine;
using TMPro; // Đảm bảo thêm namespace này nếu bạn dùng TextMeshPro

public class PlayerBalance : MonoBehaviour
{
    public float balanceSpeed = 50f; // Tốc độ nghiêng người
    public float maxBalanceAngle = 30f; // Góc nghiêng tối đa trên mặt đất
    public float resetSpeed = 5f; // Tốc độ trả về vị trí cân bằng
    public float jumpForce = 5f; // Lực nhảy của nhân vật
    public float doubleJumpForce = 7f; // Lực nhảy cho lần nhảy thứ hai. Tăng giá trị này để nhảy cao hơn.
    public float doubleJumpTimeWindow = 0.5f; // Thời gian cho phép thực hiện double jump
    public float flipSpeed = 720f; // Tốc độ xoay 360 độ (độ/giây)

    [Header("Ground Alignment")]
    public float groundAlignmentSpeed = 5f; // Tốc độ nhân vật xoay theo độ dốc mặt đất

    [Header("Mid-air Control")]
    public float midAirBalanceSpeedMultiplier = 0.5f; // Hệ số nhân tốc độ cân bằng trên không
    public float maxMidAirBalanceAngle = 45f; // Góc nghiêng tối đa trên không

    [Header("Free Fall Control")]
    public float freeFallBalanceDriftSpeed = 10f; // Tốc độ mất cân bằng khi rơi tự do và không có input

    [Header("Game Over Condition")]
    public GameObject gameOverPanel; // Kéo thả UI Panel Game Over vào đây
    public float fallOverRotationSpeed = 300f; // Tốc độ quay khi ngã xuống (độ/giây). Dùng cho Slerp
    public float fallAnimationDuration = 1.0f; // Thời gian để nhân vật lộn vòng hết trước khi game over
    public float maxLandingAngle = 60f; // Góc nghiêng tối đa cho phép khi tiếp đất

    [Header("Particle Effects")]
    public ParticleSystem dustParticles; // Kéo Dust Particles vào đây
    public ParticleSystem crashEffect; // Kéo Crash Effect vào đây
    public ParticleSystem landingEffect; // Kéo hiệu ứng đáp đất vào đây

    [Header("Scoring")]
    public TextMeshProUGUI scoreText; // Kéo TextMeshProUGUI hiển thị điểm vào đây
    public TextMeshProUGUI greatEffectText; // Kéo TextMeshProUGUI hiển thị "Great" vào đây
    public float scorePerSecond = 10f; // Điểm tăng mỗi giây
    public int doubleJumpTrickPoints = 1000; // Điểm thưởng cho cú double jump 360 thành công
    public float greatEffectDuration = 1.0f; // Thời gian hiển thị hiệu ứng "Great"
    public float perfectLandingBonusAngle = 10f; // Góc nghiêng tối đa cho phép để nhận điểm bonus khi tiếp đất hoàn hảo
    public float maxPerfectLandingAngularVelocity = 50f; // Tốc độ góc tối đa cho phép để nhận điểm bonus khi tiếp đất hoàn hảo

    [Header("Score Animation")]
    public float scorePulseScale = 1.2f; // Tỷ lệ phóng to khi nhấp nháy
    public float scorePulseDuration = 0.2f; // Thời gian của mỗi lần nhấp nháy (một nửa chu kỳ)
    public Color normalScoreColor = Color.white; // Màu điểm số bình thường
    public Color pulseScoreColor = Color.yellow; // Màu điểm số khi nhấp nháy

    private long score = 0; // Biến lưu điểm số
    private float greatEffectTimer = 0f;
    private bool doubleJumpFlipCompleted = false; // Cờ theo dõi hoàn thành flip 360

    private float rawScore = 0f; // Biến lưu điểm số chính xác hơn

    private float currentBalanceRotation = 0f;
    private Rigidbody2D rb; // Tham chiếu đến Rigidbody2D của nhân vật
    public Transform groundCheck; // Đối tượng dùng để kiểm tra mặt đất
    public LayerMask groundLayer; // Layer của mặt đất
    public float groundCheckRadius = 0.2f; // Bán kính kiểm tra mặt đất

    private bool isGameOver = false;
    private bool isFallingOver = false;
    private float fallOverTimer = 0f;
    private Quaternion startFallRotation; // Góc quay ban đầu khi bắt đầu ngã
    private Quaternion targetFallRotation; // Góc quay mục tiêu khi ngã
    private bool wasGrounded; // Theo dõi trạng thái mặt đất ở frame trước

    // Biến cho double jump và flip
    private bool canDoubleJump = false;
    private float lastJumpTime = 0f;
    private bool isFlipping = false;
    private float currentFlipRotationAmount = 0f; // Sử dụng để tích lũy góc quay của cú nhào lộn
    private float initialFlipAngle; // Góc quay ban đầu của rigidbody khi bắt đầu nhào lộn
    private bool hasDoubleJumped = false; 

    // Biến cho animation điểm số thủ công
    private float scoreAnimationTimer = 0f;

    // Biến để lưu trữ góc nghiêng khi tiếp đất
    private float landingBalanceAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on PlayerBalance object!");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f; 
        wasGrounded = IsGrounded(); 
        hasDoubleJumped = false;
        score = 0; // Khởi tạo điểm số
        rawScore = 0f; // Khởi tạo rawScore

        // Đảm bảo các hệ thống hạt và hiệu ứng UI bắt đầu ở trạng thái dừng/ẩn
        if (dustParticles != null) dustParticles.Stop();
        if (crashEffect != null) crashEffect.Stop();
        if (landingEffect != null) landingEffect.Stop();
        if (greatEffectText != null) greatEffectText.gameObject.SetActive(false);
        UpdateScoreDisplay(); // Cập nhật hiển thị điểm số ban đầu

        // Đặt màu điểm số ban đầu
        if (scoreText != null) scoreText.color = normalScoreColor;
    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        // Tăng điểm theo thời gian
        rawScore += scorePerSecond * Time.deltaTime; // Tích lũy điểm chính xác
        score = (long)rawScore; // Cập nhật điểm hiển thị
        UpdateScoreDisplay();

        // Quản lý hiển thị hiệu ứng "Great"
        if (greatEffectText != null && greatEffectText.gameObject.activeSelf)
        {
            greatEffectTimer -= Time.deltaTime;
            if (greatEffectTimer <= 0f)
            {
                greatEffectText.gameObject.SetActive(false);
            }
        }

        // Animation điểm số thủ công
        if (scoreText != null)
        {
            scoreAnimationTimer += Time.deltaTime;
            float t = Mathf.PingPong(scoreAnimationTimer, scorePulseDuration) / scorePulseDuration;

            // Animation scale
            float currentScale = Mathf.Lerp(1f, scorePulseScale, t);
            scoreText.transform.localScale = Vector3.one * currentScale;

            // Animation màu sắc
            scoreText.color = Color.Lerp(normalScoreColor, pulseScoreColor, t);
        }

        if (isFallingOver)
        {
            fallOverTimer += Time.deltaTime;
            float t = Mathf.Min(1f, fallOverTimer / fallAnimationDuration);
            transform.rotation = Quaternion.Slerp(startFallRotation, targetFallRotation, t);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y);

            if (fallOverTimer >= fallAnimationDuration)
            {
                TriggerGameOver(); 
            }
            return;
        }

        // --- Normal Balance Control ---
        float horizontalInput = Input.GetAxis("Horizontal");
        bool currentlyGrounded = IsGrounded();

        // Xử lý Dust Particles
        if (dustParticles != null)
        {
            if (currentlyGrounded)
            {
                if (!dustParticles.isPlaying) dustParticles.Play();
            }
            else
            {
                if (dustParticles.isPlaying) dustParticles.Stop();
            }
        }

        // Reset double jump khi chạm đất
        if (currentlyGrounded && !wasGrounded) // Vừa chạm đất
        {
            Debug.Log("Đã chạm đất - Reset double jump");
            canDoubleJump = false;
            hasDoubleJumped = false;
            isFlipping = false; // Đảm bảo ngừng flip khi chạm đất
            currentFlipRotationAmount = 0f;
            landingBalanceAngle = rb.rotation; // LƯU GÓC QUAY THỰC TẾ CỦA RIGIDBODY KHI TIẾP ĐẤT

            // Chơi hiệu ứng đáp đất
            if (landingEffect != null) landingEffect.Play();

            // Kiểm tra nếu cú double jump 360 độ được hoàn thành thành công và tiếp đất an toàn
            if (doubleJumpFlipCompleted) // Cú flip đã hoàn thành trong không trung
            {
                float actualLandingTilt = Mathf.Abs(Mathf.DeltaAngle(landingBalanceAngle, 0f));
                Debug.Log($"Flip completed. Current Balance Rotation on landing: {actualLandingTilt} vs Perfect Bonus Angle: {perfectLandingBonusAngle} vs Max Landing Angle: {maxLandingAngle} vs Angular Velocity: {Mathf.Abs(rb.angularVelocity)}");
                // Kiểm tra góc nghiêng và tốc độ góc khi tiếp đất để nhận bonus
                if (actualLandingTilt <= perfectLandingBonusAngle && Mathf.Abs(rb.angularVelocity) <= maxPerfectLandingAngularVelocity)
                {
                    rawScore += doubleJumpTrickPoints; // Cộng điểm thưởng vào rawScore
                    if (greatEffectText != null)
                    {
                        greatEffectText.text = $"X{doubleJumpTrickPoints} POINT PERFECT!"; // Hiển thị tin nhắn "Perfect!"
                        greatEffectText.gameObject.SetActive(true);
                        greatEffectTimer = greatEffectDuration;
                    }
                    Debug.Log("Perfect landing! Bonus points awarded.");
                }
                else if (actualLandingTilt <= maxLandingAngle)
                {
                    Debug.Log("Good landing, but not perfect enough for bonus points.");
                    // Không cộng bonus ở đây
                }
                else // Nếu currentBalanceRotation > maxLandingAngle, TriggerGameOver sẽ xử lý ở phần kiểm tra sau.
                {
                    Debug.Log("Landing too unbalanced. No bonus, potentially game over.");
                }

                doubleJumpFlipCompleted = false; // Reset cờ sau khi kiểm tra
            }
            else
            {
                Debug.Log("Double jump flip not completed before landing.");
            }
        }

        float currentBalanceSpeed = balanceSpeed; 
        float currentMaxBalanceAngle = maxBalanceAngle; 

        if (!currentlyGrounded)
        {
            currentBalanceSpeed *= midAirBalanceSpeedMultiplier;
            currentMaxBalanceAngle = maxMidAirBalanceAngle;
        }

        if (horizontalInput != 0)
        {
            currentBalanceRotation -= horizontalInput * currentBalanceSpeed * Time.deltaTime;
            currentBalanceRotation = Mathf.Clamp(currentBalanceRotation, -currentMaxBalanceAngle, currentMaxBalanceAngle);
        } else {
            if (currentlyGrounded)
            {
                currentBalanceRotation = Mathf.Lerp(currentBalanceRotation, 0f, resetSpeed * Time.deltaTime);
            }
            else // Not grounded and no horizontal input (free fall)
            {
                // Drift logic for free fall
                if (currentBalanceRotation >= 0)
                {
                    currentBalanceRotation += freeFallBalanceDriftSpeed * Time.deltaTime;
                }
                else
                {
                    currentBalanceRotation -= freeFallBalanceDriftSpeed * Time.deltaTime;
                }
                currentBalanceRotation = Mathf.Clamp(currentBalanceRotation, -currentMaxBalanceAngle, currentMaxBalanceAngle);
            }
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentlyGrounded)
            {
                Debug.Log("Nhảy lần đầu");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = true;
                hasDoubleJumped = false;
                doubleJumpFlipCompleted = false; // Reset cờ khi nhảy lần đầu
                lastJumpTime = Time.time;
            }
            else if (canDoubleJump && !hasDoubleJumped && Time.time - lastJumpTime <= doubleJumpTimeWindow)
            {
                Debug.Log("Double Jump!");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
                canDoubleJump = false;
                hasDoubleJumped = true;
                
                // Bắt đầu nhào lộn
                isFlipping = true;
                currentFlipRotationAmount = 0f; // Reset góc quay cho cú lộn
                initialFlipAngle = rb.rotation; // Ghi lại góc quay ban đầu của rigidbody
            }
            else
            {
                Debug.Log($"Không thể double jump: canDoubleJump={canDoubleJump}, hasDoubleJumped={hasDoubleJumped}, Time since last jump={Time.time - lastJumpTime}");
            }
        }

        // --- Check for Landing Condition (Game Over if bad landing) ---
        if (currentlyGrounded && !wasGrounded) 
        {
            float actualLandingTilt = Mathf.Abs(Mathf.DeltaAngle(landingBalanceAngle, 0f));
            // Sử dụng landingBalanceAngle để kiểm tra Game Over khi tiếp đất
            if (actualLandingTilt > maxLandingAngle)
            {
                TriggerGameOver(); 
            }
        }

        // --- Check for Oversteer Trigger (Starts the fall animation) ---
        if (Mathf.Abs(currentBalanceRotation) > maxBalanceAngle && !isFallingOver)
        {
            isFallingOver = true;
            fallOverTimer = 0f; 
            startFallRotation = transform.rotation; 

            float targetAngle = (currentBalanceRotation > 0) ? 180f : -180f;
            targetFallRotation = Quaternion.Euler(0, 0, targetAngle);
        }

        wasGrounded = currentlyGrounded; 
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return; 
        }

        if (isFallingOver)
        {
            return;
        }

        // Tính toán targetRotation dựa trên căn chỉnh mặt đất và currentBalanceRotation
        Quaternion targetRotation;

        if (IsGrounded())
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius + 0.2f, groundLayer);
            if (hit.collider != null)
            {
                Vector2 groundNormal = hit.normal;
                Quaternion groundAlignment = Quaternion.FromToRotation(Vector3.up, groundNormal);
                targetRotation = groundAlignment * Quaternion.Euler(0, 0, currentBalanceRotation);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, currentBalanceRotation);
            }
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, currentBalanceRotation);
        }

        // Xử lý nhào lộn trong FixedUpdate để tương tác với vật lý
        if (isFlipping)
        {
            currentFlipRotationAmount += flipSpeed * Time.fixedDeltaTime;
            if (currentFlipRotationAmount >= 360f) // Đã hoàn thành 360 độ
            {
                isFlipping = false;
                currentFlipRotationAmount = 0f;
                doubleJumpFlipCompleted = true; // Đánh dấu đã hoàn thành flip
                // Không đặt lại rb.rotation ở đây. Để logic cân bằng bình thường tiếp quản.
            }
            else
            {
                // Tiếp tục xoay nhân vật trong quá trình flip
                float newAngle = initialFlipAngle + currentFlipRotationAmount;
                rb.MoveRotation(newAngle);
            }
        }
        else // Không nhào lộn (hoặc vừa kết thúc nhào lộn trong frame này)
        {
            // Áp dụng cân bằng bình thường và căn chỉnh mặt đất
            rb.MoveRotation(Quaternion.Slerp(Quaternion.Euler(0, 0, rb.rotation), targetRotation, groundAlignmentSpeed * Time.fixedDeltaTime).eulerAngles.z);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        isFallingOver = false; 

        Debug.Log("Game Over! Player lost balance!");

        Time.timeScale = 0f; 
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        transform.rotation = targetFallRotation; 

        // Chơi hiệu ứng Crash Effect khi Game Over
        if (crashEffect != null) crashEffect.Play();

        // Dừng các hiệu ứng hạt khác khi game over
        if (dustParticles != null) dustParticles.Stop();
        if (landingEffect != null) landingEffect.Stop();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("D6");
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Vector3 currentUp = transform.up; 
        
        Quaternion leftLimitRotation = Quaternion.Euler(0, 0, maxBalanceAngle);
        Quaternion rightLimitRotation = Quaternion.Euler(0, 0, -maxBalanceAngle);

        Vector3 leftLimitDir = leftLimitRotation * currentUp;
        Vector3 rightLimitDir = rightLimitRotation * currentUp;

        Gizmos.DrawLine(transform.position, transform.position + leftLimitDir * 1f);
        Gizmos.DrawLine(transform.position, transform.position + rightLimitDir * 1f);

        if (isFallingOver)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (targetFallRotation * Vector3.up) * 1f);
        }
    }
} 