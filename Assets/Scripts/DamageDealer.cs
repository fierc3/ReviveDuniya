using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public bool damageOn; // dano esta ativo
    public float damageAmount; // quantia de dano que ira causar no player
    //public float increaseOnPhase2; // valor que sera adicionado ao dano na fase 2
    public AudioClip[] impactSound; // som que fara ao impactar com alguma coisa

    private float lastSoundTime = 0;

    public float GetDamage() // caso algun script queira saber o quanto de dano esse objeto causa
    {
        return damageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("pum-" + other.gameObject.tag);
        if (!damageOn) return; // retorna caso nao possa causar dano

        if (/*other.gameObject.layer != 9 && */other.gameObject.layer != 11 && other.gameObject.layer != 13) return; // nao atinge o que nao for da layer Ground, Player ou Scenary

        if (other.gameObject.tag == "Player") // caso tenha colidido com o player
        {
            if (other.GetComponentInParent<HumanAnimationController>().IsStunned()) return;
            other.transform.GetComponentInParent<PlayerController>().RegisterDamage(damageAmount, this.gameObject);
        }

        if (SoundInterval() && impactSound.Length > 0) // caso ja deu o intervalo para poder gerar som novamente
        {
            SoundManager.CreateAndPlay(impactSound[Random.Range(0, impactSound.Length)], GameObject.FindGameObjectWithTag("SoundManager").gameObject, other.transform, 2); // toca o som de impacto
            lastSoundTime = Time.time;
        }
    }

    public void GreatSwordFiller(GameObject other)
    {
        Debug.Log("pum-2" + other.gameObject.tag);
        if (!damageOn) return; // retorna caso nao possa causar dano

        if (other.gameObject.layer != 11 && other.gameObject.layer != 13) return; // nao atinge o que nao for da layer Ground, Player ou Scenary

        if (other.gameObject.tag == "Player") // caso tenha colidido com o player
        {
            if (other.GetComponent<Animator>().GetBool("Intangible")) return; // nao faz dano e nem som caso o player nao possa ser acertado
            other.transform.GetComponentInParent<PlayerController>().RegisterDamage(damageAmount, this.gameObject); // infringe o dano no player
        }

        if (SoundInterval() && impactSound.Length > 0) // caso ja deu o intervalo para poder gerar som novamente
        {
            SoundManager.CreateAndPlay(impactSound[Random.Range(0, impactSound.Length)], GameObject.FindGameObjectWithTag("SoundManager").gameObject, other.transform, 2); // toca o som de impacto
            lastSoundTime = Time.time;
        }
    }

    private bool SoundInterval()
    {
        return Time.time > lastSoundTime + 0.5f;
    }

}
