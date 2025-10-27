"""Tests for resume parser"""
import pytest
from parsers.resume_parser import AdvancedResumeParser

def test_skill_extraction():
    """Test skill extraction"""
    parser = AdvancedResumeParser(use_spacy=False)
    
    sample_text = "I have experience with Python, TensorFlow, and AWS cloud services."
    skills = parser.extract_skills(sample_text)
    
    assert 'python' in skills
    assert 'tensorflow' in skills
    assert 'aws' in skills

def test_education_extraction():
    """Test education parsing"""
    parser = AdvancedResumeParser(use_spacy=False)
    
    sample_text = "Master of Science in Computer Science from MIT, 2020"
    education = parser.extract_education(sample_text)
    
    assert len(education) > 0

if __name__ == "__main__":
    pytest.main([__file__, "-v"])