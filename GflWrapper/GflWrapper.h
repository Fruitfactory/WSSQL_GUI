// GflWrapper.h

#pragma once

#include <libgfl.h>
#include <libgfle.h>

#include <msclr/marshal.h>
#include <strsafe.h>

#pragma comment(lib, "gdi32.lib") 

#using <System.Windows.Forms.dll>
using namespace msclr::interop;
using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

namespace GflWrapper {

	public ref class GflImageWrapper
	{
	private:
		String^ _fileName;

		char* GetCString(System::String^ text)
		{
			
			cli::array<wchar_t> ^chArray = text->ToCharArray();
			cli::array<unsigned char, 1> ^arr = System::Text::Encoding::ASCII->GetBytes(chArray);

			char *c_arr = new char[arr->Length + 1];
			memset(c_arr, NULL, arr->Length + 1);

			System::IntPtr c_arr_ptr(c_arr);

			System::Runtime::InteropServices::Marshal::Copy(arr, 0, c_arr_ptr, arr->Length);

			return c_arr;
		}

	public:

		GflImageWrapper(String^ filename)
		{
			_fileName = filename;
		}


		bool IsSupportExt()
		{
		 	GFL_ERROR error =  gflLibraryInit();
			if(error || String::IsNullOrEmpty(_fileName))
				return false;
			String^ ext = Path::GetExtension(_fileName)->Replace(".","")->ToLowerInvariant();
			
			char* extChar = GetCString(ext);
			bool res = false;// gflFormatIsSupported(extChar);

			int count = gflGetNumberOfFormat();
			for(int i = 0;i < count;i++)
			{
				const char* extName = gflGetFormatNameByIndex(i);
				if(!strcmp(extChar,extName) || !strcmp(extChar,"jpg"))
				{
					res = true;
					break;
				}
				//free((void*)extName);
			}

			gflLibraryExit();
			free(extChar);
			return res;
		}

		Bitmap^ GetImage()
		{
			GFL_ERROR error = gflLibraryInit();
			if(error)
				return nullptr;
			GFL_LOAD_PARAMS load_option;
			GFL_BITMAP* bitmap;
			GFL_FILE_INFORMATION file_info;
			Bitmap^ bmp = nullptr;

			gflGetDefaultLoadParams(&load_option);
			char* filename = GetCString(_fileName);
			gflGetFileInformation(filename,-1,&file_info);
			error = gflLoadBitmap(filename,&bitmap,&load_option,&file_info);
			if(!error)
			{
				HBITMAP hBmp = nullptr;
				//convert to hbitmap, because if we copy pixels data to managed bitmap -  color madness;
				gflConvertBitmapIntoDDB(bitmap,&hBmp);

				IntPtr ptrHBmp =  IntPtr(hBmp);

				bmp = Bitmap::FromHbitmap(ptrHBmp);
				DeleteObject(hBmp);
			}

			free(filename);
			gflFreeBitmap(bitmap);
			gflLibraryExit();

			return bmp;
		}


	};
}
