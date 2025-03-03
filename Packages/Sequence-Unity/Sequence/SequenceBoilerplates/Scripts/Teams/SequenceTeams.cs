using System;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates.Teams
{
    public class SequenceTeams : MonoBehaviour
    {
        private enum State
        {
            Overview,
            Create,
            Details
        }
        
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private GameObject _overviewState;
        [SerializeField] private GameObject _createState;
        [SerializeField] private GameObject _teamDetailsState;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GenericObjectPool<SequenceTeamTile> _teamTilePool;
        [SerializeField] private GenericObjectPool<SequenceTeamMemberTile> _memberTilePool;

        private Action _onClose;
        private TeamsApi _api;
        private State _state;
        
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }

        public void Show(IWallet wallet, Chain chain, string apiUrl, Action onClose = null)
        {
            gameObject.SetActive(true);
            _onClose = onClose;
            _api = new TeamsApi(wallet, apiUrl);
            
            SetState(State.Overview);
            LoadTeams();
        }

        public void ShowOverviewState()
        {
            SetState(State.Overview);
        }

        public void ShowCreateState()
        {
            SetState(State.Create);
        }

        public async void CreateTeam()
        {
            var teamName = _nameInput.text;
            var description = _descriptionInput.text;

            SetLoading(true);
            await _api.CreateTeam(teamName, description, "", "");
            SetLoading(false);
            
            SetState(State.Overview);
            LoadTeams();
        }

        private async void LoadTeams()
        {
            _teamTilePool.Cleanup();
            var teams = await _api.GetTeams();
            
            foreach (var team in teams)
                _teamTilePool.GetObject().Load(team, () => ShowTeam(team));
        }

        private async void ShowTeam(TeamData team)
        {
            SetState(State.Details);

            _nameText.text = team.name;
            _descriptionText.text = team.description;
            
            _memberTilePool.Cleanup();
            var members = await _api.GetMembers(team.uid);
            
            foreach (var member in members)
                _memberTilePool.GetObject().Load(member);
        }

        private void SetState(State state)
        {
            _state = state;
            _overviewState.SetActive(state == State.Overview);
            _createState.SetActive(state == State.Create);
            _teamDetailsState.SetActive(state == State.Details);
            _backButton.SetActive(state != State.Overview);
        }

        private void SetLoading(bool enable)
        {
            _loadingScreen.SetActive(enable);
        }
    }
}