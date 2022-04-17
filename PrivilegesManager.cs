namespace archlinux_aip
{
    internal class PrivilegesManager
    {
        public static bool IsUserAdmin(string username)
        {
            bool is_admin = false;

            if (username == "root")
            {
                is_admin = true;
            }
            else
            {
                is_admin = false;
            }
            return is_admin;
        }
    }
}
