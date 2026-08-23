using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteToSolidOrange : MonoBehaviour
{
    public ParticleSystem _hitEffect; // 受击粒子系统
    public ParticleSystem _hitSmokeEffect; //受击烟雾粒子系统
    
    public Material _orangeMat; // 赋值方式2创建的橙色纯色材质
    private Coroutine co;
    public float fadeDuration = 0.5f; // 橙色褪去时长（秒），可自定义
    public Color darkOrangeColor = new Color(0.8f, 0.3f, 0f, 1f); // 深橙，可Inspector调节
    private static readonly int FadeFactor = Shader.PropertyToID("_FadeFactor"); // Shader属性ID（优化性能）
    private static readonly int OrangeColor = Shader.PropertyToID("_OrangeColor");
    void Awake()
    {
        _orangeMat = new Material(_orangeMat);
        /*_orangeMat.SetFloat(FadeFactor, 0f);*/
        /*_orangeMat.SetColor(OrangeColor, darkOrangeColor);*/
    }

    void Update()
    {
        if (_hitSmokeEffect.isPlaying == false && _hitSmokeEffect != null)
        {
            PoolManager.Instance.Push("Prefabs/Effects/HitSmoke",_hitSmokeEffect.gameObject);
        }
    }

    // 变成纯橙色（保留原图片形状/透明）
    public void TriggerHitEffect(Transform attackPosition)
    {
        Vector2 direction = ((Vector2)transform.position - (Vector2)attackPosition.position).normalized;
        float angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (co != null) //图片变为纯橙色
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(LerpFromOrangeToOriginal());
        _hitEffect.Play(); //橙色光圈
        PoolManager.Instance.Get("Prefabs/Effects/HitSmoke", (o) =>
        {
            _hitSmokeEffect = o.GetComponent<ParticleSystem>();
            _hitSmokeEffect.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
            _hitSmokeEffect.transform.rotation = Quaternion.Euler(angle, 90, 0);
            _hitSmokeEffect.Play();
        });
        Bleed();//血液飞溅
    }

    private IEnumerator LerpFromOrangeToOriginal()
    {
        // 瞬间重置为深橙色（系数0）
        _orangeMat.SetFloat(FadeFactor, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // 0→1的平滑系数，可选线性/缓动（推荐SmoothStep丝滑）
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsedTime / fadeDuration));
            // 仅修改Shader的褪色系数，无其他操作
            _orangeMat.SetFloat(FadeFactor, t);
            yield return null; // 每帧更新
        }
    }

    private void Bleed()
    {
        for (int i = 0; i < 10; i++)
        {
            PoolManager.Instance.Get("Prefabs/Enemy/Other/Blood", (o) =>
            {
                o.transform.position = new Vector3(transform.position.x, transform.position.y, -22);
                o.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-10, 10f), Random.Range(10, 15f)),ForceMode2D.Impulse);
            });
        }
    }
}
