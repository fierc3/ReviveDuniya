using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BossLifeBarScript : MonoBehaviour
{

    public float maxLife = 40; // maximo de vida possivel
    [SerializeField]
    private float life = 0; // total de vida
    private float filler = 30; // valor pelo qual a vida sera multiplicada
    private float ghost = 0; // ghost da barra de vida
    private int barHeight = 17; // altura da barra de vida
    public Animator bossAnim; // animator do boss


    private float lastTime;
    private float waitTime = 1.5f;

    [HideInInspector]
    public bool fillBossLifeBar = false;

    [Header("Win")]
    public GameObject winnerScreen;
    public GameObject bonfire;
    public GameObject winEffect;
    public AudioSource musicSource;

    public AchievementManager achievementManager;
    public LifeBarScript playerLifeBarScript;
    public PlayerController playerController;
    public GameManagerScript gameManager;

    private void Start()
    {
        life = maxLife; // inicia com a vida cheia mas ainda nao atualiza a exibicao na tela
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && gameManager.master)
        {
            UpdateLife(-10);
        }
    }

    public void UpdateLife(float amount)
    {
        Debug.Log("pum2 aaa " + life);
        if (IsDead()) return; // nao faz nada caso esteja morto

        if (amount < 0) // caso esteja decrementando a vida
        {
            lastTime = Time.time;
        }

        life += amount; // realiza a mudanca na vida

        if (life > maxLife) life = maxLife; // garante que ela nao seja maior que o permitido
        if (life < 0) life = 0;// garante que ela nao seja menor que o permitido

        Debug.Log("pum2 b " + life);

        if (life == 0 && !IsDead()) // mata o boss caso ainda nao tenha feito
        {
            Die();
        }
    }

    private bool IsDead() // retorna se o boss esta morto
    {
        return bossAnim.GetBool("Dead");
    }

    private void Die() // mata o boss
    {
        bossAnim.SetBool("Dead", true); // seta o boss como morto
        bossAnim.SetFloat("Vertical", 0); // para o movimento do boss
        bossAnim.SetFloat("Horizontal", 0); // para o movimento do boss
                                            //  StartCoroutine(AfterWin());
                                            //  GameManagerScript.isBossDead = true; // seta o boss como morto, usado para parar a musica
        Invoke(nameof(CleanUp), 2f);
    }

    private void CleanUp()
    {
        GameObject.Destroy(this.gameObject.transform.parent.gameObject.transform.parent.transform.parent.gameObject);
    }

    public float GetBossLifeAmount() // retorna a quantia de vida do boss
    {
        return life;
    }

    /*
    IEnumerator AfterWin()
    {
        StartCoroutine(FadeMusic());
        yield return new WaitForSeconds(1.5f); // espera um pouco ate o boss cair
        Vector3 offset = new Vector3(0, 0, 1);
        Instantiate(winEffect, bossAnim.gameObject.transform.position + offset, quaternion.identity); // efeito de onda de choque
        winnerScreen.SetActive(true); // escrita de sucesso
        bonfire.SetActive(true); // ativa o bonfire
        bossAnim.gameObject.GetComponent<CapsuleCollider>().isTrigger = true; // desativa o collider do boss depois que ele morre
        CapsuleCollider[] legs = bossAnim.gameObject.GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider leg in legs) leg.isTrigger = true; // desativa o collider das pernas ao morrer
        achievementManager.TriggerDefeatBoss(); // achievement de vitoria
        if (playerLifeBarScript.GetNoDamageTaken()) achievementManager.TriggerNoDamageTaken(); // confere se venceu sem receber dano
        if (playerLifeBarScript.GetEstusFlaskAmount() == 5) achievementManager.TriggerNoHeals(); // confere se venceu sem receber dano
        playerController.DisableEstusFlask(); // desativa o estus para o player poder ascender o bonfire
        musicSource.volume = 0; // assegura que nao havera mais musica
        this.gameObject.SetActive(false); // desativa a barra de vida
    }

    /*
    IEnumerator FadeMusic()
    {
        yield return new WaitForSeconds(0.25f);
        musicSource.volume -= 0.225f;
        if(musicSource.volume > 0)
            StartCoroutine(FadeMusic());
    }
    */

}
