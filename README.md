Multiple Passwords Encrypter
====
�����p�X���[�h�Í����\�t�g�E�F�A

## Description
���̒��̑����̃V�X�e���Ńp�X���[�h��p�X�t���[�Y�ɂ��F�؂��g���Ă��܂��B  
�������p�X���[�h�͕����񒷂╶����ɐ�������������A�����������Ƃ��Ă��L�����ɂ����A����ɔ����ĕ����V�X�e���œ����p�X���[�h�̎g���܂킵�Ȃǂ����ƂȂ�܂��B  
�p�X���[�h�Ɏ������̂Ƃ��āA�p�X���[�h��Y�ꂽ�ۂ̃��Z�b�g�Ĕ��s�Ȃǂ̍ۂɁu�閧�̎���v���g���邱�Ƃ�����܂��B  
�u���Ȃ��̍D���ȃt���[�c�́H�v�Ƃ��u�y�b�g�̖��O�́H�v�Ƃ���������ɉ񓚂�o�^���Ă��������ł��B  
�������������^�̎��₵���Ȃ����Ƃ�����܂����A�܂��u�D���ȃt���[�c�v�ɉ񓚂���ɂ��Ă��u�ь�v���u���v���������炢�D���Ƃ����ꍇ��A
�u�����S�v�œo�^�����̂��u��񂲁v�Ȃ̂��A�͂��܂��uapple�v�Ȃ̂��Y��Ă��܂����Ƃ�����܂��B  
�p�X�t���[�Y�ł��uImagine all the people�v�Ƃ�����߂ɂ��悤�Ǝv���Ă��A�ŏ���啶���Ŏn�߂��̂��S�����������Y��Ă��܂�����A�P��ԂɊԈႦ�ăX�y�[�X��2����邾���ŔF�؂Ɏ��s���܂��B

�܂Ƃ߂�ƁA�p�X���[�h�F�؂ɂ�  
- ��̃t���[�Y�����ݒ�ł��Ȃ�
- �\�L�������e���Ȃ�
�Ƃ�������肪����܂��B�t�ɍl����΁A
- �����̃t���[�Y��ݒ�ł���
- �\�L�������e����
�p�X���[�h�F�؃V�X�e�����悢�ƍl���A���̎����̗�Ƃ��Ă��̔F�ؕ����𓋍ڂ����Í����\�t�g�E�F�A���������܂����B


## User Interface
�����ł�3�̎����ݒ肵�ăt�@�C�����Í��������ƁA���̃t�@�C���𕜍�������ۂ�UI���Ƃ��ċ����܂����B  
�ŏ��̎���ł́u��񂲁v�u�����S�v�u���v�Ȃǂ̉񓚂����e���܂��B  
�������̂悤�ȕ\�L�������e����Ƃ������Ƃ́A���̕��p�X���[�h�̋��x�������邱�Ƃ��Ӗ����܂��B  
�����ŕ����̎����ݒ�\�Ƃ��A�����ł́u3�̎���̂���2�ȏ㐳������ƕ����ł���v�Ƃ����ݒ�ɂ��邱�Ƃŋ@�������グ�Ă��܂��B
(3�̎���ɑS�Đ������Ȃ��ƕ����ł��Ȃ��Ƃ����ݒ���\�ł�)


## Download

���̃\�t�g�E�F�A�͖��ۏ؂ł��B�o�b�N�A�b�v��ۑ������ɏd�v���̍����t�@�C�����Í�������̂̓��X�N�����邱�Ƃ������m���������B  
(�s��񍐂͊��}�������܂�)

## Licence
This software is distributed under the MIT License.  
���̃\�t�g�E�F�A�� MIT License �̂��ƂŔz�z�������܂��B



## �Í����̗���
�ŏ��ɈÍ����Ώۂ̃t�@�C���� Zip �A�[�J�C�u������ 1 �t�@�C���ɂ��܂�(�ȍ~data)�B  
256bit���� �ō쐬���� �Í����L�[(dataKey)��p���āAdata �� AES-256 �ňÍ������܂��B  
	data + dataKey => encryptedData

���Ɏ��₲�ƂɈÍ����L�[(questionKey)�� 256bit���� �ō쐬���܂��B  
��L��̂悤�Ɏ��₪3����ꍇ�́AquestionKey0 �` questionKey2 �𐶐����܂��B  
���Ɏ���̐��Ɛ����̐��̑g�ݍ��킹�� dataKey ���Í�������L�[(combinedQuestionKey)���쐬���܂��B  
��L��̂悤��3�̎���̂���2�𐳓�����K�v������ꍇ�́A3C2 �� 3�ʂ�̑g�ݍ��킹�ƂȂ�܂��B  
- combinedQuestionKey0_1 = questionKey0 + questionKey1
- combinedQuestionKey0_2 = questionKey0 + questionKey2
- combinedQuestionKey1_2 = questionKey1 + questionKey2
(�����ł�+�͒P�Ɍ������Ӗ����܂�)

����3�̎���̂���3�𐳓�����K�v������ꍇ�́AdataKey ���Í�������L�[�͈ȉ��̈�����ƂȂ�܂��B  
combinedQuestionKey0_1_2 = questionKey0 + questionKey1 + questionKey2

combinedQuestionKey ��p���āAdataKey ���Í������܂��B  
	dataKey + combinedQuestionKey0_1 => encryptedDataKey0_1
	dataKey + combinedQuestionKey0_2 => encryptedDataKey0_2
	dataKey + combinedQuestionKey1_2 => encryptedDataKey1_2

�Ō�ɁA���[�U�[�����͂����p�X���[�h�� questionKey ���Í������܂��B  
��̂悤�ɁA�ŏ��̎���ɓ�̃p�X���[�h���ݒ肳��Ă���ꍇ�A���ꂼ��̃p�X���[�h�ňÍ������܂��B  
	questionKey0 + "���" -> encryptedQuestionKey0-0
	questionKey0 + "�Ȃ�" -> encryptedQuestionKey0-1

�u�p�X���[�h�Ɋ܂܂��󔒂𖳎�����v�u�啶���Ə���������ʂ��Ȃ��v�Ȃǂ̃I�v�V�������ݒ肳��Ă���ꍇ�́A
�p�X���[�h����������̂悤�ɐ��K�����ĈÍ������܂��B(�������̍ۂ����̓p�X���[�h�𓯗l�ɐ��K�����Ďg�p����)

��̃t�@�C���� encryptedData�AencryptedDataKey0_1 ...�AencryptedQuestionKey0-0 ... ���L�^���ĈÍ����t�@�C���Ƃ��܂��B



## �������̗���
����0�ł̓��[�U�[�����͂����p�X���[�h�ŁAencryptedQuestionKey0-0�AencryptedQuestionKey0-1�A�c�Ə��ɕ��������݂Ă����܂��B  
�����ł���΂��̎���� questionKey0 �����o���܂��B  
����0 �� ����2 �������ł���΁A questionKey0 + questionKey2 = combinedQuestionKey0_2 ���擾�ł��܂��̂ŁAencryptedDataKey0_2 ���� dataKey ���擾�ł��܂��B  
	encryptedDataKey0_2 - combinedQuestionKey0_2 => dataKey
dataKey ��p���� data �������ł��܂��B  
	encryptedData - dataKey => data



## �Í��t�@�C���t�H�[�}�b�g

file format
constant				3	�萔 "MPE"
archive format version	3	�Í��t�@�C���t�H�[�}�b�g�̃o�[�W�����B"000"
encoder version			3	�G���R�[�_�[�̃o�[�W�����B"000"
Question header len		4	Question header �\���̂̃T�C�Y
Question header CRC32	4	Question header �\���̂� CRC
encryptedData len		8	�Í����f�[�^�̃T�C�Y
encryptedData CRC32		4	�Í����f�[�^�� CRC
Question header			(1)	Question header �\����
encryptedData(*)		n	�Í����f�[�^


Question header
	flag					1
		isTrim				0x01
		isRemoveSpace		0x02
		isIgnoreCase		0x04
		isIgnoreZenHan		0x08
		isIgnoreHiraKata	0x10
		isNoCompress		0x80
	nQuestions				4
	nRequiredPasswords		4
	nDataKeyCombinations	4	(nQuestions �� nRequiredPasswords ����v�Z���ł��邪�A�ꉞ�ێ�)
	encryptedDataKeys(*)	nDataKeyCombinations * 48
	Question				(nQuestions)	nQuestions * Question �\����

Question
	hint string len				4
	hint string					n
	nPasswords					4
	encryptedQuestionKeys(*)	nPasswords * 48




