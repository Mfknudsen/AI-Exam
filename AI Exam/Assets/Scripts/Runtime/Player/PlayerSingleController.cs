#region Libraries

using UnityEngine;

#endregion

namespace Runtime.Player
{
    public sealed class PlayerSingleController : PlayerController
    {
        public override void OnEpisodeBegin()
        {
            this.transform.position = this.transform.root.position +
                                      new Vector3(Random.Range(-0.5f, -9f), 0, Random.Range(-9f, 9f));
            this.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            this.transform.root.GetChild(0).position = this.transform.root.position +
                                                       new Vector3(Random.Range(0.5f, 7f), .5f, Random.Range(-9f, 9f));
            this.transform.root.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                this.SetReward(-5);
                Debug.Log(this.GetCumulativeReward());
                this.EndEpisode();
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
                this.SetReward(5);
        }
    }
}