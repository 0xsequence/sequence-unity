using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Teams
{
    public class SequenceTeamTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _icon;

        private UnityAction _onClick;
        
        public void Load(TeamData team, UnityAction onClick)
        {
            _onClick = onClick;
            _nameText.text = team.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }
    }
}