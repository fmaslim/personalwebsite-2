from pdfminer.high_level import extract_text
infile = r"c:\GitHub\PersonalWebsite\PersonalWebsite.Api\Controllers\Files\hello_world_api_prd.pdf"
outfile = r"c:\GitHub\PersonalWebsite\PersonalWebsite.Api\Controllers\Files\hello_world_api_prd_extracted.txt"
text = extract_text(infile)
open(outfile,'w',encoding='utf-8').write(text)
print('EXTRACTED')
