using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_ActiveSessions
    {
        public static Dictionary<Guid, TMUser> activeSessions(this TM_Xml_Database tmDb)
        {
            return TM_Xml_Database.Current.UserData.ActiveSessions;
        }		        
        public static Guid              registerUserSession  (this string userName, Guid userGuid)
        {
            var tmUser = userName.tmUser();			
            return tmUser.registerUserSession(userGuid);
        }
        public static Guid              registerUserSession  (this string userName, Guid userGuid, int groupId)
        {
            var tmUser = userName.tmUser();
            tmUser.GroupID = groupId;
            return tmUser.registerUserSession(userGuid);
        }
        public static Guid              registerUserSession  (this TMUser tmUser, Guid userGuid)
        {
            try
            {
                if (tmUser.notNull() && userGuid != Guid.Empty)
                {
                    tmUser.logUserActivity("User Login", tmUser.UserName);
                    TM_UserData.Current.ActiveSessions.add(userGuid, tmUser);
                    return userGuid;
                }
            }
            catch (Exception ex)
            {
                ex.log();
            }
            return Guid.Empty;
        }                                
        public static bool              validSession         (this Guid sessionId)
        {
            return TM_UserData.Current.ActiveSessions.hasKey(sessionId);
        }
        public static bool              invalidateSession    (this Guid sessionId)
        {
            if (sessionId.validSession())
            {
                sessionId.session_TmUser().logUserActivity("User Logout", sessionId.session_UserName());
                TM_UserData.Current.ActiveSessions.Remove(sessionId);
                return true;
            }
            return false;
        }        
        public static TMUser            session_TmUser       (this Guid sessionId)
        {
            if(sessionId.validSession())
                return TM_UserData.Current.ActiveSessions[sessionId];
            return null;	
        }
        public static string            session_UserName     (this Guid sessionId)
        {			
            if(sessionId.validSession())
                return sessionId.session_TmUser().UserName;
            return null;
        }
        public static int               session_GroupID      (this Guid sessionId)
        { 
            var tmUser = sessionId.session_TmUser();
            if (tmUser != null)
                return tmUser.GroupID;
            return -1;            
        }        
        public static UserGroup         session_UserGroup    (this Guid sessionId)
        {
            return (UserGroup)sessionId.session_GroupID();              
        }        
        public static List<UserRole>    session_UserRoles    (this Guid sessionId)
        {
            var userGroup = sessionId.session_UserGroup();
            if (UserRolesMappings.Mappings.hasKey(userGroup))
                return UserRolesMappings.Mappings[userGroup];
            return new List<UserRole>();
        }        
        public static bool              session_isAdmin      (this Guid sessionId)
        {
            return UserGroup.Admin == sessionId.session_UserGroup();
        }  

    }
}