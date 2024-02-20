using System;
using System.Threading.Tasks;
using Sequence.WaaS.Authentication;

namespace Sequence.WaaS.Tests
{
    public class SessionManagementTests
    {
        private WaaSWallet _wallet;
        
        public SessionManagementTests(WaaSWallet wallet)
        {
            _wallet = wallet;
        }

        public async Task TestSessionManagement()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                WaaSSession[] sessions = await TestListSessions();
                if (sessions == null)
                {
                    return;
                }

                await TestDropInactiveSession(sessions);
                
                await _wallet.DropThisSession();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSessionManagement), e.Message));
                return;
            }

            try
            {
                WaaSSession[] sessions = await _wallet.ListSessions();
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSessionManagement), "Expected an exception. Session was likely not dropped."));
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestPassed?.Invoke();
            }
        }

        private async Task<WaaSSession[]> TestListSessions()
        {
            try
            {
                WaaSSession[] sessions = await _wallet.ListSessions();
                if (sessions == null || sessions.Length == 0)
                {
                    throw new Exception("No sessions found.");
                }

                return sessions;
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestListSessions),
                    e.Message));
                return null;
            }
        }

        private async Task TestDropInactiveSession(WaaSSession[] sessions)
        {
            try
            {
                int sessionCount = sessions.Length;
                for (int i = 0; i < sessionCount; i++)
                {
                    if (sessions[i].id != _wallet.SessionId)
                    {
                        await _wallet.DropSession(sessions[i].id);
                        break;
                    }
                }

                if (sessionCount > 1)
                {
                    sessions = await TestListSessions();
                    int newSessionCount = sessions.Length;
                    CustomAssert.IsTrue(newSessionCount == sessionCount - 1, nameof(TestSessionManagement));
                }
            }
            catch(Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestDropInactiveSession), e.Message));
            }
        }
    }
}