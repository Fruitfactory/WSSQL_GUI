///////////////////////////////////////////////////////////
//  FileContentSearchRule.cs
//  Implementation of the Class FileContentSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:49 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


namespace WSUI.Infrastructure.Implements.Rules 
{
	public class FileContentSearchRule : BaseFilelSearchRule 
    {

		public FileContentSearchRule()
		{
		    ConstructorInit();
		}

        public FileContentSearchRule(object lockObject)
            :base(lockObject)
        {
            ConstructorInit();
        }


	    private void ConstructorInit()
	    {
	        Priority = 5;
	        WhereTemplate =
                " WHERE scope='file:' AND Contains({0}) AND System.DateCreated < '{1}' ORDER BY System.DateCreated DESC";
	    }

//System.Kind <> 'email' AND System.Kind <> 'folder' AND System.Kind <> 'contact' AND




	    public override void Init()
	    {
            RuleName = "FileContent";
	        base.Init();
	    }
    }//end FileContentSearchRule

}//end namespace Implements