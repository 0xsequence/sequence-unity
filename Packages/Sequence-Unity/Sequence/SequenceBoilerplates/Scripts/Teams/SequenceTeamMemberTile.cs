using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Sequence.Boilerplates.Teams
{
    public class SequenceTeamMemberTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;

        private UnityAction _onClick;
        
        public void Load(TeamMemberData member)
        {
            _nameText.text = member.walletAddress;
        }
    }
}