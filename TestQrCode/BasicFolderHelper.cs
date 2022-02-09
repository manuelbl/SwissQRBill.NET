
namespace TestQrCode
{


    public class BasicFolderHelper
    {


        // https://stackoverflow.com/a/19968118/155077
        public static System.Collections.Generic.List<string> EnumFolders(string path)
        {
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            System.Collections.Generic.Stack<string> stack = new System.Collections.Generic.Stack<string>();
            stack.Push(path);


            int folderCount = 0;
            int fileCount = 0;

            while (stack.Count != 0)
            {
                folderCount++;
                string currentPath = stack.Pop();

                try
                {
                    string[] files = System.IO.Directory.GetFiles(currentPath, "*", System.IO.SearchOption.TopDirectoryOnly);

                    System.Array.Sort(files,
                        delegate (string a, string b)
                        {
                            if (a == null && b == null)
                                return 0;

                            // if (a == null) return -1; // NULL to top // if (b == null) return 1; // not necessary - already that way in a.CompareTo(b);
                            if (a == null) return 1; if (b == null) return -1; // NULL to bottom

                            return a.CompareTo(b);
                        }
                    );

                    for (int i = 0; i < files.Length; ++i)
                    {
                        string ext = System.IO.Path.GetExtension(files[i]);

                        if (".cs".Equals(System.IO.Path.GetExtension(ext), System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            fileCount++;
                            ls.Add(files[i]);
                        }
                    }

                }
                catch (System.Exception)
                { }

                try
                {
                    string[] children = System.IO.Directory.GetDirectories(currentPath);

                    System.Array.Sort(children, 
                        delegate (string a, string b) 
                        { 
                            return a.CompareTo(b); 
                        }
                    );


                    for (int i = children.Length - 1; i > -1; --i)
                    {
                        stack.Push(children[i]);
                    }

                }
                catch (System.Exception)
                { }

            } // Whend 

            return ls;
        } // End Function ListFolders 


    }


}
