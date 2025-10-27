"""Tests for feedback generation"""
import pytest
from feedback.ai_feedback_generator import AIFeedbackGenerator

def test_fallback_feedback():
    """Test rule-based feedback generation"""
    generator = AIFeedbackGenerator(api_key=None)  
    resume_data = {
        'skills': ['python', 'sql'],
        'experience': [{'title': 'Developer'}],
        'education': [{'degree': 'BS CS'}]
    }
    
    job_data = {
        'required_skills': ['python', 'java', 'aws'],
        'preferred_skills': []
    }
    
    score_details = {
        'overall_score': 65.0,
        'missing_critical_skills': ['java', 'aws']
    }
    
    feedback = generator._generate_fallback_feedback(
        resume_data,
        job_data,
        score_details
    )
    
    assert len(feedback.strengths) > 0
    assert len(feedback.missing_skills) > 0

if __name__ == "__main__":
    pytest.main([__file__, "-v"])