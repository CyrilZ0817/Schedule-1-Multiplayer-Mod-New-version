#nullable disable
using MelonLoader;
using HarmonyLib;
using System;

[assembly: MelonInfo(typeof(ScheduleMod.MultiplayerPlus), "Schedule I Multiplayer Plus", "1.0.0", "cyrilz")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ScheduleMod
{
	public class MultiplayerPlus : MelonMod
	{
		// Define the target player limit (Steam maximum is 250, but 16 is stable for this game)
		private const int MAX_PLAYERS = 16;

		public override void OnInitializeMelon()
		{
			var harmony = new HarmonyLib.Harmony("com.cyrilz.schedule1.multiplayerplus");

			// --- Steamworks API Patches ---

			// Forces the lobby creation to request 16 slots instead of 4
			Patch(harmony, "Il2CppSteamworks.SteamMatchmaking:CreateLobby", nameof(Prefix_CreateLobby), null);

			// Ensures the game identifies the lobby limit as 16 when querying metadata
			Patch(harmony, "Il2CppSteamworks.SteamMatchmaking:GetLobbyMemberLimit", null, nameof(Postfix_ReturnLimit));

			// CORE BYPASS: Always returns 1 when the game asks for the current member count.
			// This prevents the "Lobby is Full" internal check from triggering on the client side.
			Patch(harmony, "Il2CppSteamworks.SteamMatchmaking:GetNumLobbyMembers", null, nameof(Postfix_FakeCount));

			// Intercepts lobby data to ensure any "max_players" or "limit" keys are synchronized to 16
			Patch(harmony, "Il2CppSteamworks.SteamMatchmaking:SetLobbyData", nameof(Prefix_SetData), null);
			Patch(harmony, "Il2CppSteamworks.SteamMatchmaking:GetLobbyData", null, nameof(Postfix_GetData));

			// Hooks the join request callback to ensure smooth transition when accepting invites
			Patch(harmony, "Il2CppSteamworks.SteamFriends:OnLobbyJoinRequested", nameof(Prefix_Void), null);

			// --- UI Patches ---

			// Dynamically updates any UI text containing "/4" to "/16" (e.g., "1/4" -> "1/16")
			var uiMethod = AccessTools.Method("TMPro.TMP_Text:set_text") ?? AccessTools.Method("Il2CppTMPro.TMP_Text:set_text");
			if (uiMethod != null)
				harmony.Patch(uiMethod, null, new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPlus), nameof(Postfix_UIRender))));
		}

		private void Patch(HarmonyLib.Harmony h, string path, string pre, string post)
		{
			var method = AccessTools.Method(path);
			if (method != null)
			{
				var prefix = pre != null ? new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPlus), pre)) : null;
				var postfix = post != null ? new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPlus), post)) : null;
				h.Patch(method, prefix, postfix);
			}
		}

		// Logic Implementations
		public static void Prefix_CreateLobby(ref int cMaxMembers) => cMaxMembers = MAX_PLAYERS;
		public static void Postfix_ReturnLimit(ref int __result) { if (__result > 0) __result = MAX_PLAYERS; }
		public static void Postfix_FakeCount(ref int __result) { if (__result >= 1) __result = 1; }
		public static void Prefix_Void() { }

		public static void Prefix_SetData(string pchKey, ref string pchValue)
		{
			if (pchValue == "4" || pchKey.ToLower().Contains("limit") || pchKey.ToLower().Contains("max"))
				pchValue = MAX_PLAYERS.ToString();
		}

		public static void Postfix_GetData(ref string __result)
		{
			if (__result == "4") __result = MAX_PLAYERS.ToString();
		}

		public static void Postfix_UIRender(object __instance, string __0)
		{
			if (!string.IsNullOrEmpty(__0) && __0.Contains("/4"))
			{
				try
				{
					var prop = __instance.GetType().GetProperty("text");
					prop?.SetValue(__instance, __0.Replace("/4", "/" + MAX_PLAYERS));
				}
				catch { }
			}
		}
	}
}