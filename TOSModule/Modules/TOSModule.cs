using System;
using System.Reflection;
using log4net;
using Mono.Addins;
using Nini.Config;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using OpenMetaverse;

namespace TOSModule
{
    public class TOSModule : INonSharedRegionModule
    {
        private bool m_Enabled;
        private string m_TOSURL;

        public string Name
        {
            get { return "TOSModule"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void Initialise(Scene scene, IConfigSource config)
        {
            IConfig moduleConfig = config.Configs["TOSModule"];
            if (moduleConfig != null)
            {
                m_Enabled = moduleConfig.GetBoolean("Enabled", false);
                m_TOSURL = moduleConfig.GetString("TOSURL", "");
            }

            if (m_Enabled)
            {
                scene.EventManager.OnNewClient += OnNewClient;
            }
        }

        public void Close()
        {
        }

        public void AddRegion(Scene scene)
        {
        }

        public void RemoveRegion(Scene scene)
        {
        }

        public void RegionLoaded(Scene scene)
        {
        }

        private void OnNewClient(IClientAPI client)
        {
            client.OnAuthenticate += OnAuthenticate;
        }

        private void OnAuthenticate(UUID agentID, string firstName, string lastName, string password, string clientVersion, string channel)
        {
            ScenePresence sp = Util.GetScenePresence(agentID);
            if (sp == null)
                return;

            if (!sp.ControllingClient.TermsOfServiceAccepted)
            {
                sp.ControllingClient.SendAlertMessage("You must accept the Terms of Service before you can log in.");
                sp.ControllingClient.SendEstateList(null);
                sp.ControllingClient.Kick("You must accept the Terms of Service before you can log in.");
            }
            else
            {
                sp.ControllingClient.SendRegionHandshake(sp.Scene.RegionInfo.RegionName, sp.Scene.RegionInfo.RegionID.Guid, sp.AbsolutePosition, sp.Lookat);
            }
        }
    }
}
